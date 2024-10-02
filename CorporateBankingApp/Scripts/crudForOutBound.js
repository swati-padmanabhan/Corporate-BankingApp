function loadOutBoundClients() {

    $.ajax({

        url: "/Client/GetAllBeneficiaries",

        type: "GET",

        success: function (data) {

            console.log(data);

            $("#beneficiaryTable").empty();

            if (data.length > 0) {

                $.each(data, function (index, beneficiary) {

                    var row = `<tr>
<td>${beneficiary.BeneficiaryName}</td>
<td>${beneficiary.AccountNumber}</td>
<td>${beneficiary.BankIFSC}</td>
<td>
<input type="checkbox" class="is-active-checkbox"

                                data-beneficiaryid="${beneficiary.Id}"

                                ${beneficiary.IsActive ? "checked" : ""} />
</td>
<td>
<button onClick="editBeneficiary('${beneficiary.Id}')" class="action-btn btn btn-secondary btn-sm" title="Edit">
<i class="bi bi-pencil-square"></i>
</button>
</td>
</tr>`;

                    $("#beneficiaryTable").append(row);

                });

                // Event listener for changing the active status

                $(".is-active-checkbox").change(function () {

                    var beneficiaryId = $(this).data("beneficiaryid");

                    var isActive = $(this).is(":checked");

                    updateBeneficiaryStatus(beneficiaryId, isActive);

                });

            } else {

                $("#beneficiaryTable").append("<tr><td colspan='5' class='text-center'>No beneficiaries found.</td></tr>");

            }

        },

        error: function (err) {

            console.log("Error Retrieving Beneficiary:", err);

        }

    });

}

function editBeneficiary(beneficiaryId) {

    getBeneficiary(beneficiaryId); // Retrieve beneficiary details

    $("#beneficiaryList").hide();

    $("#editRecord").show();

}

function getBeneficiary(beneficiaryId) {

    $.ajax({

        url: "/Client/GetBeneficiaryById",

        type: "GET",

        data: { id: beneficiaryId },

        success: function (response) {

            if (response.success) {

                $("#editBeneficiaryId").val(response.beneficiary.Id);

                $("#newBeneficiaryName").val(response.beneficiary.BeneficiaryName);

                $("#newAccountNumber").val(response.beneficiary.AccountNumber);

                $("#newBankIFSC").val(response.beneficiary.BankIFSC);

            } else {

                alert(response.message);

            }

        },

        error: function (err) {

            alert("No Such Data Found");

        }

    });

}

$("#btnEdit").click(() => {

    var data = {

        Id: $("#editBeneficiaryId").val(),

        BeneficiaryName: $("#newBeneficiaryName").val(),

        AccountNumber: $("#newAccountNumber").val(),

        BankIFSC: $("#newBankIFSC").val(),

    };

    modifyRecord(data);

});

function modifyRecord(modifiedBeneficiary) {

    $.ajax({

        url: "/Client/EditBeneficiaries",

        type: "POST",

        data: modifiedBeneficiary,

        success: function (response) {

            if (response.success) {

                alert("Beneficiary Edited Successfully");

                loadOutBoundClients();

                $("#beneficiaryList").show();

                $("#editRecord").hide();

            } else {

                alert(response.message);

            }

        },

        error: function (err) {

            alert("Error Editing Beneficiary Record");

        }

    });

}

function addBeneficiary(beneficiaryId) {
    $.ajax({
        url: '/Client/MakePaymentRequests', // Change this to the correct route
        type: 'POST',
        data: { beneficiaryId: beneficiaryId },
        success: function (response) {
            // Display success message or redirect to Make Payment page
            window.location.href = '/Client/MakePaymentRequests'; // Redirect to the Make Payment page
        },
        error: function (xhr, status, error) {
            // Handle error
            alert('Error adding beneficiary for payment: ' + error);
        }
    });
}

function updateBeneficiaryStatus(beneficiaryId, isActive) {

    $.ajax({

        url: "/Client/UpdateBeneficiaryStatus",

        type: "POST",

        data: { Id: beneficiaryId, isActive: isActive },

        success: function (response) {

            if (response.success) {

                showModal(

                    "success",

                    "Beneficiary Status Updated",

                    `Beneficiary's status has been ${isActive ? "activated" : "deactivated"}.`

                );

            } else {

                showModal(

                    "error",

                    "Error updating beneficiary status",

                    "Something went wrong."

                );

            }

        },

        error: function (err) {

            console.log("Error Updating Beneficiary Status: ", err);

        },

    });

}

// Optional: To handle adding a new beneficiary

$("#btnAdd").click(function () {

    $("#beneficiaryList").hide();

    $("#newRecord").show();

});

