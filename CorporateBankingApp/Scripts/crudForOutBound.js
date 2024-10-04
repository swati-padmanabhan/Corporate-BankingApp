let beneficiariesData = [];
let currentPage = 1;
const pageSize = 2;

function loadOutboundBeneficiaries() {
    $.ajax({
        url: "/Client/GetAllOutboundBeneficiaries",
        type: "GET",
        success: function (data) {
            beneficiariesData = data;
            renderTable(currentPage);
            setupPagination();
        },
        error: function (err) {
            $("#beneficiaryTblBody").empty();
            $("#warningNotice").show(); // Show warning notice on error
            console.error("Error fetching beneficiaries:", err);
        }
    });
}
function renderTable(page) {
    $("#beneficiaryTblBody").empty();
    const start = (page - 1) * pageSize;
    const end = start + pageSize;
    const paginatedData = beneficiariesData.slice(start, end);

    if (paginatedData.length > 0) {
        $.each(paginatedData, function (index, item) {
            var documentLinks = item.DocumentPaths.map(function (url) {
                var fileName = url.split('/').pop();
                return `<a href="#" class="document-link" data-url="${url}">${fileName}</a>`;
            }).join("<br/> ");

            var row = `<tr>
                    <td>
                        <div class="d-flex align-items-center">
                            <img src="https://ui-avatars.com/api/?name=${encodeURIComponent(item.BeneficiaryName)}&background=4e4187&color=ffffff" alt="" style="width: 40px; height: 40px" class="rounded-circle" />
                            <div class="ms-3">
                                <p class="fw-bold mb-1">${item.BeneficiaryName}</p>
                            </div>
                        </div>
                    </td>
                    <td>
                        <p class="fw-normal mb-1">${item.AccountNumber}</p>
                    </td>
                    <td>${item.BankIFSC}</td>
                    <td>
                        <span class="badge ${item.BeneficiaryStatus === 'PENDING' ? 'bg-warning' : 'bg-success'} rounded-pill d-inline">${item.BeneficiaryStatus}</span>
                    </td>
                    <td>
                        <span class="badge ${item.BeneficiaryType === 'INBOUND' ? 'primary-bg neutral-light-text' : 'bg-secondary'} rounded-pill d-inline">${item.BeneficiaryType}</span>
                    </td>
                    <td>
                        <div class="d-flex align-items-center">
                            <input type="checkbox" id="toggle-${item.Id}" data-beneficiaryid="${item.Id}" class="toggle" ${item.IsActive ? "checked" : ""} />
                            <label for="toggle-${item.Id}" class="toggle-label"></label>
                        </div>
                    </td>
                    <td>
                        <div class="open-document">
                            ${documentLinks}
                        </div>
                    </td>
                    <td class="editbeneficiary-btn-cell">
                        <i class="bi bi-pencil-square edit-icon" onclick="editBeneficiary('${item.Id}')" style="${item.IsActive ? '' : 'display:none;'}"></i>
                    </td>
                </tr>`;

            $("#beneficiaryTblBody").append(row);
            $(".toggle").change(function () {
                var beneficiaryId = $(this).data("beneficiaryid");
                var isActive = $(this).is(":checked");
                updateBeneficiaryStatus(beneficiaryId, isActive);
            });
        });
    } else {
        $("#warningNotice").show();
    }
}
function setupPagination() {
    const totalPages = Math.ceil(beneficiariesData.length / pageSize);
    let paginationHtml = '';

    for (let i = 1; i <= totalPages; i++) {
        paginationHtml += `<li class="page-item ${i === currentPage ? 'active' : ''}">
            <a class="page-link" href="#" onclick="loadPage(${i})">${i}</a>
        </li>`;
    }

    $("#pagination").html(paginationHtml);
}
function loadPage(page) {
    currentPage = page;
    renderTable(currentPage);
}
function updateBeneficiaryStatus(beneficiaryId, isActive) {
    $.ajax({
        url: "/Client/UpdateBeneficiaryStatus",
        type: "POST",
        data: { Id: beneficiaryId, isActive: isActive },
        success: function (response) {
            if (response.success) {

                alert("Beneficiary status updated successfully");

                // Find the row containing the beneficiary using the toggle ID
                var beneficiaryRow = $(`#beneficiaryTblBody tr`).filter(function () {
                    return $(this).find(`#toggle-${beneficiaryId}`).length > 0;
                });

                var editButton = beneficiaryRow.find(".editbeneficiary-btn-cell .edit-icon");

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
    // Clear previous error messages
    $(".error-message").remove();

    // Validate fields
    var isValid = true;

    // Validate Beneficiary Name
    var beneficiaryName = $("#newBeneficiaryName").val();
    if (!beneficiaryName) {
        isValid = false;
        $("#newBeneficiaryName").after('<span class="error-message" style="color:red;">Beneficiary Name is required.</span>');
    }

    // Validate Account Number
    var accountNumber = $("#newAccountNumber").val();
    var accountNumberPattern = /^\d{12}$/;
    if (!accountNumber || !accountNumberPattern.test(accountNumber)) {
        isValid = false;
        $("#newAccountNumber").after('<span class="error-message" style="color:red;">Account Number must be 12 digits.</span>');
    }

    // Validate Bank IFSC
    var bankIFSC = $("#newBankIFSC").val();
    var bankIFSCPattern = /^[A-Z]{4}0[A-Z0-9]{6}$/;
    if (!bankIFSC || !bankIFSCPattern.test(bankIFSC)) {
        isValid = false;
        $("#newBankIFSC").after('<span class="error-message" style="color:red;">Invalid Bank IFSC format.</span>');
    }

    // Validate File Inputs
    var addressProofFile = $("#BeneficiaryAddressProof")[0].files[0];
    var idProofFile = $("#BeneficiaryIdProof")[0].files[0];
    if (!addressProofFile) {
        isValid = false;
        $("#BeneficiaryAddressProof").after('<span class="error-message" style="color:red;">Beneficiary Address Proof is required.</span>');
    }
    if (!idProofFile) {
        isValid = false;
        $("#BeneficiaryIdProof").after('<span class="error-message" style="color:red;">Beneficiary ID Proof is required.</span>');
    }

    // If any field is invalid, stop the submission
    if (!isValid) {
        return; // Stop further execution
    }

    // If all fields are valid, proceed with form submission
    var formData = new FormData();
    formData.append("BeneficiaryName", beneficiaryName);
    formData.append("AccountNumber", accountNumber);
    formData.append("BankIFSC", bankIFSC);
    formData.append("uploadedDocs1", idProofFile);
    formData.append("uploadedDocs2", addressProofFile);

    console.log(formData);

    $.ajax({
        url: "/Client/AddNewBeneficiary",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                alert("New Beneficiary added successfully");
                loadOutboundBeneficiaries();
                $("#addNewBeneficiary").hide();
                $("#beneficiaryList").show();
            } else {
                alert("Error adding new Beneficiary: " + response.errors.join(", "));
            }
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