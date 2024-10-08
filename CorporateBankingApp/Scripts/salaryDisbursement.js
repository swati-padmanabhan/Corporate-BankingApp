var selectedIds = [];

// Get selected checkboxes
function getSelectedDisbursements() {
    selectedIds = []; // Reset selectedIds on each call
    $(".salary-checkbox:checked").each(function () {
        selectedIds.push($(this).val());
    });
    return selectedIds;
}

// Approve selected disbursements
$('#approveSelected').click(function () {
    var selectedIds = getSelectedDisbursements();
    if (selectedIds.length === 0) {
        alert("Please select at least one salary disbursement.");
        return;
    }

    $.ajax({
        type: "POST",
        url: '/Admin/ApproveDisbursements',
        data: { disbursementIds: selectedIds },
        traditional: true,
        beforeSend: function () {
            $('#loadingIndicator').show(); // Show loading indicator
        },
        success: function (response) {
            $('#loadingIndicator').hide(); // Hide loading indicator
            if (response.success) {
                setTimeout(function () {
                    alert(response.message);
                    // Remove approved rows from the table
                    selectedIds.forEach(function (id) {
                        $('#salaryDisbursementsTable tr[data-id="' + id + '"]').remove();
                    });
                }, 100); // Delay to allow loading indicator to hide
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            $('#loadingIndicator').hide(); // Hide loading indicator
            alert("An error occurred while approving the disbursements: " + error);
        }
    });
});

// Reject selected disbursements
$('#rejectSelected').click(function () {
    var selectedIds = getSelectedDisbursements();
    if (selectedIds.length === 0) {
        alert("Please select at least one salary disbursement.");
        return;
    }

    $.ajax({
        type: "POST",
        url: '/Admin/RejectDisbursements',
        data: { disbursementIds: selectedIds },
        traditional: true,
        beforeSend: function () {
            $('#loadingIndicator').show(); // Show loading indicator
        },
        success: function (response) {
            $('#loadingIndicator').hide(); // Hide loading indicator
            if (response.success) {
                setTimeout(function () {
                    alert(response.message);
                    // Remove rejected rows from the table
                    selectedIds.forEach(function (id) {
                        $('#salaryDisbursementsTable tr[data-id="' + id + '"]').remove();
                    });
                }, 100); // Delay to allow loading indicator to hide
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            $('#loadingIndicator').hide(); // Hide loading indicator
            alert("An error occurred while rejecting the disbursements: " + error);
        }
    });
});