$(document).ready(function () {
    window.approveDisbursement = function (salaryDisbursementId) {
        $.ajax({
            type: "POST",
            url: '/Admin/ApproveDisbursement', // Adjust if necessary
            data: { salaryDisbursementId: salaryDisbursementId },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    // Optionally refresh the disbursement list or reload the page
                    location.reload();
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                alert("An error occurred while approving the disbursement: " + error);
            }
        });
    };

    window.rejectDisbursement = function (salaryDisbursementId) {
        $.ajax({
            type: "POST",
            url: '/Admin/RejectDisbursement', // Adjust if necessary
            data: { salaryDisbursementId: salaryDisbursementId },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    // Optionally refresh the disbursement list or reload the page
                    location.reload();
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                alert("An error occurred while rejecting the disbursement: " + error);
            }
        });
    };
});

