let currentFilters = {
    status: null
};

function loadBeneficiaryForVerification() {
    // Prepare the AJAX URL based on the current filter
    let url = "/Admin/GetOutboundBeneficiaryForVerification";

    if (currentFilters.status) {
        url += `?status=${currentFilters.status}`; // Add the status filter to the URL if applicable
    }

    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            $("#beneficiaryToBeVerifiedTblBody").empty();
            console.log(data);
            if (data.length === 0) {
                var noBeneficiariesMessage = `<tr><td colspan="7" class="text-center">No beneficiaries left to be verified</td></tr>`;
                $("#beneficiaryToBeVerifiedTblBody").append(noBeneficiariesMessage);
            } else {
                $.each(data, function (index, item) {
                    // Create a list of document links
                    var documents = item.DocumentPaths.map(function (docPath) {
                        var fileName = docPath.split('/').pop(); // Extract file name from path
                        return `<a href="#" class="open-document" data-filepath="${docPath}" target="_blank">${fileName}</a><br>`;
                    }).join('');

                    var row = `<tr>
                        <td>${item.BeneficiaryName}</td>
                        <td>${item.AccountNumber}</td>
                        <td>${item.BankIFSC}</td>
                        <td>
                        <span class="badge ${item.BeneficiaryType === 'INBOUND' ? 'primary-bg neutral-light-text' : 'bg-secondary'} rounded-pill d-inline">${item.BeneficiaryType}</span>
                        </td>
                        <td>${documents}</td>
                        <td><div class="d-flex">
                            <button onclick="approveBeneficiary('${item.Id}', 'APPROVED')" class="btn btn-success mx-3"> <i class="bi bi-check"></i>Approve</button>
                            <button onclick="rejectBeneficiary('${item.Id}', 'REJECTED')" class="btn btn-danger"> <i class="bi bi-x"></i>Reject</button>
                        </div></td>
                    </tr>`;
                    $("#beneficiaryToBeVerifiedTblBody").append(row);
                });

                // Add click event to open modal and display document
                $(".open-document").click(function (e) {
                    e.preventDefault();
                    var filePath = $(this).data('filepath');

                    // Load the document in the iframe
                    $('#documentFrame').attr('src', filePath);

                    // Show the modal
                    $('#documentModal').modal('show');
                });
            }
        },
        error: function (err) {
            $("#beneficiaryToBeVerifiedTblBody").empty();
            var errorMessage = `<tr><td colspan="7" class="text-center">No beneficiaries waiting to be verified</td></tr>`;
            $("#beneficiaryToBeVerifiedTblBody").append(errorMessage);
            alert("Error occurred while loading beneficiaries for verification.");
        }
    });
}

// Function to filter by status
function filterByStatus(status) {
    currentFilters.status = status; // Set the current filter
    loadBeneficiaryForVerification(); // Load beneficiaries with the applied filter
}

// Function to reset filters
function filterAll() {
    currentFilters.status = null; // Reset the status filter
    loadBeneficiaryForVerification(); // Load all beneficiaries
}

//document modal closing button
$('.close').on('click', function () {
    $('#documentModal').modal('hide');
});

// Use the same function for both approve and reject buttons
function approveBeneficiary(beneficiaryId) {
    updateBeneficiaryStatus(beneficiaryId, 'APPROVED');
}

function rejectBeneficiary(beneficiaryId) {
    updateBeneficiaryStatus(beneficiaryId, 'REJECTED');
}
// Combined function for approving or rejecting client
function updateBeneficiaryStatus(beneficiaryId, status) {
    $.ajax({
        url: "/Admin/UpdateOutboundBeneficiaryOnboardingStatus",
        type: "POST",
        data: { id: beneficiaryId, status: status }, // Pass the status (APPROVED or REJECTED)
        success: function () {
            alert(`Beneficiary ${status === 'APPROVED' ? 'Approved' : 'Rejected'}`);
            loadBeneficiaryForVerification();
        },
        error: function () {
            alert(`Failed to ${status === 'APPROVED' ? 'Approve' : 'Reject'} the Beneficiary`);
        }
    });
}



