function openModal(title, imageUrl) {
    console.log("modal clicked");
    console.log(title, imageUrl);

    // Set the modal title
    $("#documentModalLabel").text(title);

    // Set the modal image source
    $("#modalImage").attr("src", imageUrl);

    // Show the modal
    $("#documentModal").modal("show");
}


//beneficiary
function filterAll() {
  const rows = document.querySelectorAll("tbody tr");
  rows.forEach((row) => {
    row.style.display = ""; // Show all rows
  });
}


function filterInbound() {
  const rows = document.querySelectorAll("tbody tr");
  rows.forEach((row) => {
    const typeCell = row.children[3].textContent; // Adjust index based on your structure
    row.style.display = typeCell.trim() === "INBOUND" ? "" : "none"; // Show only INBOUND
  });
}

function filterOutbound() {
  const rows = document.querySelectorAll("tbody tr");
  rows.forEach((row) => {
    const typeCell = row.children[3].textContent; // Adjust index based on your structure
    row.style.display = typeCell.trim() === "OUTBOUND" ? "" : "none"; // Show only OUTBOUND
  });
}



// user detail page --> search filter
function filterTable(columnIndex) {
    const input = document.querySelectorAll("#search-row input")[columnIndex];
    const filter = input.value.toLowerCase();
    const table = document.getElementById("data-table");
    const rows = table.getElementsByTagName("tr");

    for (let i = 2; i < rows.length; i++) {
        // Start at 2 to skip header rows
        const cells = rows[i].getElementsByTagName("td");
        const cellValue = cells[columnIndex].textContent.toLowerCase();
        if (cellValue.includes(filter)) {
            rows[i].style.display = ""; // Show row
        } else {
            rows[i].style.display = "none"; // Hide row
        }
    }
}

//edit client document model
function getDocument(filePath) {
    document.getElementById('documentFrame').src = filePath;
}

// Password show and hide
$(".passwordIcon").on("click", () => {
    const passwordInput = $(".password");
    const passwordIcon = $(".passwordIcon");

    if (passwordInput.attr("type") === "password") {
        passwordInput.attr("type", "text"); // Show password
        passwordIcon.removeClass("bi-eye-slash").addClass("bi-eye"); // Change to eye icon
    } else {
        passwordInput.attr("type", "password"); // Hide password
        passwordIcon.removeClass("bi-eye").addClass("bi-eye-slash"); // Change to eye-slash icon
    }
});