function loadOutboundBeneficiaries() {
    $.ajax({
        url: "/Client/GetAllOutboundBeneficiaries",
        type: "GET",
        success: function (data) {
            $("#beneficiaryTblBody").empty();
            $("#warningNotice").hide(); // Hide the warning notice initially

            if (data.length > 0) {
                $.each(data, function (index, item) {
                    var documentLinks = item.DocumentPaths.map(function (url) {
                        var fileName = url.split('/').pop();
                        return `<a href="#" class="document-link" data-url="${url}">${fileName}</a>`;
                    }).join("<br/> ");

                    var row = `<tr>
                        <td>${item.BeneficiaryName}</td>
                        <td>${item.AccountNumber}</td>
                        <td>${item.BankIFSC}</td>
                        <td>${item.BeneficiaryStatus}</td>
                        <td>${item.BeneficiaryType}</td>
                        <td>
                            <input type="checkbox" class="is-active-checkbox" data-beneficiaryid="${item.Id}" ${item.IsActive ? "checked" : ""} />
                        </td>
                        <td>${documentLinks}</td>
                        <td class="editbeneficiary-btn-cell">
        <i class="bi bi-pencil-square edit-icon" onclick="editBeneficiary('${item.Id}')" style="${item.IsActive ? '' : 'display:none;'}"></i>
    </td>
                    </tr>`;

                    $("#beneficiaryTblBody").append(row);
                });

                $(".is-active-checkbox").change(function () {
                    var beneficiaryId = $(this).data("beneficiaryid");
                    var isActive = $(this).is(":checked");
                    updateBeneficiaryStatus(beneficiaryId, isActive);
                });
            } else {
                $("#warningNotice").show(); // Show warning notice if no beneficiaries
            }
        },
        error: function (err) {
            $("#beneficiaryTblBody").empty();
            $("#warningNotice").show(); // Show warning notice on error
            console.error("Error fetching beneficiaries:", err);
        }
    });
}




$(document).on("click", ".document-link", function (e) {
    e.preventDefault(); // Prevent the default link behavior
    var url = $(this).data("url"); // Get the URL from the data attribute
    console.log("Document URL:", url); // Log the URL to the console for debugging
    $("#documentFrame").attr("src", url); // Set the src of the iframe
    $("#documentModal").modal("show"); // Show the modal
});

//isactive updation
function updateBeneficiaryStatus(beneficiaryId, isActive) {
    $.ajax({
        url: "/Client/UpdateBeneficiaryStatus",
        type: "POST",
        data: { Id: beneficiaryId, isActive: isActive },
        success: function (response) {
            if (response.success) {
                alert("Beneficiary status updated successfully");

                // Find the row containing the employee
                var beneficiaryRow = $(`#beneficiaryTblBody tr`).filter(function () {
                    return $(this).find(".is-active-checkbox").data("beneficiaryid") == beneficiaryId;
                });

                var editButton = beneficiaryRow.find(".editbeneficiary-btn-cell button");

                if (isActive) {
                    editButton.show();
                } else {
                    editButton.hide();
                }
            } else {
                alert("Error updating beneficiary status: " + response.message);
            }
        },
        error: function (err) {
            console.log("Error updating beneficiary status:", err);
        }
    });
}


//add new beneficiary

function addNewBeneficiary() {
    var formData = new FormData();

    // Append normal input fields
    formData.append("BeneficiaryName", $("#newBeneficiaryName").val());
    formData.append("AccountNumber", $("#newAccountNumber").val());
    formData.append("BankIFSC", $("#newBankIFSC").val());

    // Append file inputs
    var addressProofFile = $("#BeneficiaryAddressProof")[0].files[0];
    var idProofFile = $("#BeneficiaryIdProof")[0].files[0];
 


    formData.append("uploadedDocs1", idProofFile);
    formData.append("uploadedDocs2", addressProofFile);

    $.ajax({
        url: "/Client/AddNewBeneficiary",
        type: "POST",
        data: formData,
        processData: false,  // Prevent jQuery from automatically transforming the data into a query string
        contentType: false,  // Prevent jQuery from setting Content-Type header; the browser will set it correctly
        success: function (response) {
            alert("New Beneficiary added successfully");
            loadOutboundBeneficiaries();
            $("#addNewBeneficiary").hide();
            $("#beneficiaryList").show();
        },
        error: function (err) {
            alert("Error adding new Beneficiary");
            console.log(err);
        }
    });
}

function saveBeneficiaryChanges() {
    var formData = new FormData();

    // Gather form data
    formData.append("Id", $("#editBeneficiaryId").val());
    formData.append("BeneficiaryName", $("#editBeneficiaryName").val());
    formData.append("AccountNumber", $("#editAccountNumber").val());
    formData.append("BankIFSC", $("#editBankIFSC").val());

    // Handle file inputs for ID proof and address proof
    var addressProofInput = $("#newAddressProof")[0]; // Change to correct input ID
    var idProofInput = $("#newIdProof")[0]; // Change to correct input ID
   
    var idProofFile = idProofInput.files[0]; // Access the first file
    formData.append("newIdProof", idProofFile); // Append the file to FormData


    var addressProofFile = addressProofInput.files[0]; // Access the first file
    formData.append("newAddressProof", addressProofFile); // Append the file to FormData


    // Send the form data with AJAX
    modifyBeneficiary(formData);
}




//edit button onclick
function editBeneficiary(beneficiaryId) {
    console.log("Edit button clicked for beneficiary:", beneficiaryId);
    getBeneficiary(beneficiaryId);
    $("#beneficiaryList").hide();
    $("#editBeneficiary").show();
}

function getBeneficiary(beneficiaryId) {
    $.ajax({
        url: "/Client/GetBeneficiaryById",
        type: "GET",
        data: { id: beneficiaryId },
        success: function (response) {
            if (response.success) {
                $("#editBeneficiaryId").val(response.beneficiary.Id);
                $("#editBeneficiaryName").val(response.beneficiary.BeneficiaryName);
                $("#editAccountNumber").val(response.beneficiary.AccountNumber);
                $("#editBankIFSC").val(response.beneficiary.BankIFSC);

                $("#beneficiaryList").hide();
                $("#editBeneficiary").show();
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            alert("No such data found");
        }
    });
}

function modifyBeneficiary(formData) {
    $.ajax({
        url: "/Client/EditBeneficiary",
        type: "POST",
        data: formData,
        contentType: false,  // Prevent jQuery from overriding the content type
        processData: false,  // Prevent jQuery from processing the data
        success: function (response) {
            if (response.success) {
                alert("Beneficiary Details Edited Successfully");
                loadOutboundBeneficiaries();
                $("#beneficiaryList").show();
                $("#editBeneficiary").hide();
            } else {
                alert(response.message);
            }

        },
        error: function (err) {
            alert("Error in Editing Beneficiary Details");
        }
    });
}