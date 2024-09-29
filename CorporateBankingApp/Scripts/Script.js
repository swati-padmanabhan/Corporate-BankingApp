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

function searchUsers() {
  const input = document.getElementById("searchInput");
  const filter = input.value.toLowerCase();
  const rows = document.querySelectorAll("tbody tr");

  rows.forEach((row) => {
    const nameCell = row
      .querySelector("td:nth-child(1) p")
      .textContent.toLowerCase(); // Adjust index if necessary
    if (nameCell.includes(filter)) {
      row.style.display = ""; // Show the row if it matches
    } else {
      row.style.display = "none"; // Hide the row if it doesn't match
    }
  });
}