document.addEventListener("DOMContentLoaded", function () {
    let logs = [];
    let folders = [];
    let monitoringPaused = false;
    let lastUpload = null;

    const landingPage = document.getElementById("landingPage");
    const dashboardPage = document.getElementById("dashboardPage");
    document.getElementById("pauseBtn").addEventListener("click", pauseMonitoring);
    document.getElementById("resumeBtn").addEventListener("click", resumeMonitoring);
    document.getElementById("startBtn").addEventListener("click", sendFolders);
    document.getElementById("clearBtn").addEventListener("click", clearLogs);
    document.getElementById("exportBtn").addEventListener("click", exportLogs);
    document.getElementById("pauseBtn").addEventListener("click", togglePause);
    document.getElementById("searchInput").addEventListener("input", renderLogs);
    document.getElementById("folderFilter").addEventListener("change", renderLogs);

    function sendFolders() {
        const input = document.getElementById("folderInput").value.trim();
        if (!input) return alert("Please enter at least one folder path.");

        folders = input.split(",").map(f => f.trim()).filter(f => f.length > 0);

        fetch("/api/sync/start", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(folders)
        })
            .then(res => {
                if (!res.ok) throw new Error("Check folder path and try again.");
                return res.text();
            })
            .then(() => {
                landingPage.style.display = "none";
                dashboardPage.style.display = "block";
                fetchLogs();
                fetchUploadStatus();
            })
            .catch(err => alert(err.message));
    }

    function togglePause() {
        const endpoint = monitoringPaused ? "/api/sync/resume" : "/api/sync/pause";
        fetch(endpoint, { method: "POST" })
            .then(() => {
                monitoringPaused = !monitoringPaused;
                document.getElementById("pauseBtn").textContent = monitoringPaused ? "Resume" : "Pause";
            });
    }

    function fetchLogs() {
        fetch("/api/sync/logs")
            .then(res => res.json())
            .then(data => {
                logs = data;
                updateFolderDropdown();
                renderLogs();
            });
    }

    function fetchUploadStatus() {
        fetch("/api/sync/uploadstatus")
            .then(res => res.json())
            .then(data => {
                lastUpload = data;
                updateUploadStatus();
            });
    }

    function updateUploadStatus() {
        const statusDiv = document.getElementById("uploadStatus");
        if (lastUpload?.file) {
            statusDiv.innerHTML = `<strong>Last Uploaded:</strong> ${lastUpload.file} <br/><strong>At:</strong> ${new Date(lastUpload.time).toLocaleString()}`;
        } else {
            statusDiv.textContent = "No files uploaded yet.";
        }
    }

    function clearLogs() {
        fetch("/api/sync/clear", { method: "POST" }).then(() => {
            logs = [];
            renderLogs();
        });
    }

    function exportLogs() {
        if (logs.length === 0) return alert("No logs to export.");
        const csv = [
            ["File Path", "Change Type", "Timestamp", "Folder Source"],
            ...logs.map(log => [
                `"${log.filePath}"`,
                log.changeType,
                new Date(log.timeStamp).toLocaleString(),
                log.folderSource
            ])
        ].map(e => e.join(",")).join("\n");

        const blob = new Blob([csv], { type: "text/csv" });
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = "logs.csv";
        a.click();
        URL.revokeObjectURL(url);
    }

    function updateFolderDropdown() {
        const filter = document.getElementById("folderFilter");
        const folders = [...new Set(logs.map(l => l.folderSource))];
        filter.innerHTML = '<option value="">All Folders</option>';
        folders.forEach(f => {
            const opt = document.createElement("option");
            opt.value = f;
            opt.textContent = f;
            filter.appendChild(opt);
        });
    }

    function renderLogs() {
        const tbody = document.getElementById("logTableBody");
        const search = document.getElementById("searchInput").value.toLowerCase();
        const selectedFolder = document.getElementById("folderFilter").value;

        let created = 0, modified = 0, deleted = 0;
        tbody.innerHTML = "";

        logs
            .filter(log =>
                (!selectedFolder || log.folderSource === selectedFolder) &&
                (!search || log.filePath.toLowerCase().includes(search))
            )
            .forEach(log => {
                const row = document.createElement("tr");
                const badgeClass = log.changeType === "Created" ? "created" :
                    log.changeType === "Modified" ? "modified" : "deleted";
                if (badgeClass === "created") created++;
                else if (badgeClass === "modified") modified++;
                else if (badgeClass === "deleted") deleted++;

                const isPreviewable = /\.(txt|pdf|png|jpg|jpeg)$/i.test(log.filePath);
                const previewLink = isPreviewable ? `<a href="file:///${log.filePath.replace(/\\/g, "/")}" target="_blank">🔍</a>` : "—";

                row.innerHTML = `
          <td>${log.filePath}</td>
          <td><span class="badge ${badgeClass}">${log.changeType}</span></td>
          <td>${new Date(log.timeStamp).toLocaleString()}</td>
          <td>${log.folderSource}</td>
          <td>${previewLink}</td>
        `;
                tbody.appendChild(row);
            });

        document.getElementById("logStats").textContent =
            `Log Summary: ${created} Created | ${modified} Modified | ${deleted} Deleted`;
        document.getElementById("lastUpdated").textContent =
            "Last Updated: " + new Date().toLocaleTimeString();
    }

    // Auto-refresh logs every 10 seconds
    setInterval(() => {
        fetchLogs();
        fetchUploadStatus(); // ✅ now also check status
    }, 10000);
;
});
function pauseMonitoring() {
    fetch("/api/sync/pause", { method: "POST" })
        .then(() => alert("Monitoring paused"))
        .catch(error => console.error("❌ Error pausing:", error));
}

function resumeMonitoring() {
    fetch("/api/sync/resume", { method: "POST" })
        .then(() => alert("Monitoring resumed"))
        .catch(error => console.error("❌ Error resuming:", error));
}
function fetchUploadStatus() {
    fetch("/api/sync/uploadstatus")
        .then(response => response.json())
        .then(data => {
            if (data && data.file && data.time) {
                const statusText = `☁️ Last Uploaded: ${data.file} at ${new Date(data.time).toLocaleString()}`;
                document.getElementById("uploadStatus").textContent = statusText;
            }
        })
        .catch(() => {
            document.getElementById("uploadStatus").textContent = "☁️ No recent upload.";
        });
}

