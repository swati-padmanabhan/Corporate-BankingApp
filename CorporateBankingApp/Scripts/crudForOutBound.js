let beneficiariesData = [];
let currentBeneficiaryPage = 1;
const beneficiaryPageSize = 2;

let currentFilters = {
    status: null, // For status filtering
};

// Function to filter by status
function filterByStatus(status) {
    currentFilters.status = status; // Set the current filter
    applyFilters(); // Apply filters
}

// Function to reset filters
function filterAll() {
    currentFilters.status = null; // Reset the status filter
    applyFilters(); // Apply filters
}

// Function to apply filters based on current settings
function applyFilters() {
    let filteredData = beneficiariesData;

    // Filter by status if applicable
    if (currentFilters.status) {
        filteredData = filteredData.filter(
            (item) => item.BeneficiaryStatus === currentFilters.status
        );
    }

    // Reset to the first page after filtering
    currentBeneficiaryPage = 1;

    // Render the filtered data
    renderTable(currentBeneficiaryPage, filteredData);
    setupBeneficiaryPagination(filteredData.length);
}

//function loadPage(page) {
//    currentBeneficiaryPage = page;
//    applyFilters(); // Reapply filters when changing page
//}

function loadPage(page) {
    currentBeneficiaryPage = page;

    // Only apply filters if there's a current filter set
    if (currentFilters.status != null) {
        applyFilters(); // Reapply filters when changing page
    } else {
        // If no filters, simply render the full list
        renderTable(currentBeneficiaryPage, beneficiariesData);
        setupBeneficiaryPagination(beneficiariesData.length);
    }
}


function loadOutboundBeneficiaries() {
    $.ajax({
        url: "/Client/GetAllOutboundBeneficiaries",
        type: "GET",
        success: function (data) {
            beneficiariesData = data;
            renderTable(currentBeneficiaryPage);
            setupBeneficiaryPagination();
        },
        error: function (err) {
            $("#beneficiaryTblBody").empty();
            $("#warningNotice").show(); // Show warning notice on error
            console.error("Error fetching beneficiaries:", err);
        }
    });
}
function renderTable(page, data = beneficiariesData) {
    $("#beneficiaryTblBody").empty();
    const start = (page - 1) * beneficiaryPageSize;
    const end = start + beneficiaryPageSize;
    const paginatedData = data.slice(start, end);

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

        });
    } else {
        $("#warningNotice").show();
    }
}
function setupBeneficiaryPagination(totalCount = beneficiariesData.length) {
    const totalPages = Math.ceil(totalCount / beneficiaryPageSize);
    let paginationHtml = '';

    for (let i = 1; i <= totalPages; i++) {
        paginationHtml += `<li class="${i === currentBeneficiaryPage ? 'active' : ''}">
            <a class="" href="#" onclick="loadPage(${i})">${i}</a>
        </li>`;
    }

    $("#beneficiaryPagination").html(paginationHtml);
}

function searchUsers() {
    const searchTerm = $('#searchInput').val().toLowerCase(); // Get the search term
    console.log(searchTerm)
    const filteredData = beneficiariesData.filter(item => {

        return (
            item.BeneficiaryName.toLowerCase().includes(searchTerm) ||
            item.AccountNumber.toLowerCase().includes(searchTerm) ||
            item.BankIFSC.toLowerCase().includes(searchTerm)
        );
    });
    console.log(filteredData)


    // Reset to the first page after filtering
    currentBeneficiaryPage = 1;

    // Render the filtered data
    renderTable(currentBeneficiaryPage, filteredData);
    setupBeneficiaryPagination(filteredData.length);
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
    // Clear previous error messages
    $(".error-message").remove();

    // Validate fields
    var isValid = true;

    // Validate Beneficiary ID
    var beneficiaryId = $("#editBeneficiaryId").val();
    if (!beneficiaryId) {
        isValid = false;
        $("#editBeneficiaryId").after('<span class="error-message" style="color:red;">Beneficiary ID is required.</span>');
    }

    // Validate Beneficiary Name
    var beneficiaryName = $("#editBeneficiaryName").val();
    if (!beneficiaryName) {
        isValid = false;
        $("#editBeneficiaryName").after('<span class="error-message" style="color:red;">Beneficiary Name is required.</span>');
    }

    // Validate Account Number
    var accountNumber = $("#editAccountNumber").val();
    var accountNumberPattern = /^\d{12}$/; // Exactly 12 digits
    if (!accountNumber || !accountNumberPattern.test(accountNumber)) {
        isValid = false;
        $("#editAccountNumber").after('<span class="error-message" style="color:red;">Account Number must be exactly 12 digits.</span>');
    }

    // Validate Bank IFSC
    var bankIFSC = $("#editBankIFSC").val();
    var bankIFSCPattern = /^[A-Z]{4}0[A-Z0-9]{6}$/; // Adjust based on your requirement
    if (!bankIFSC || !bankIFSCPattern.test(bankIFSC)) {
        isValid = false;
        $("#editBankIFSC").after('<span class="error-message" style="color:red;">Invalid Bank IFSC format.</span>');
    }

    // Validate File Inputs
    var addressProofInput = $("#newAddressProof")[0];
    var idProofInput = $("#newIdProof")[0];

    var idProofFile = idProofInput.files[0];
    if (!idProofFile) {
        isValid = false;
        $("#newIdProof").after('<span class="error-message" style="color:red;">Beneficiary ID Proof is required.</span>');
    }

    var addressProofFile = addressProofInput.files[0];
    if (!addressProofFile) {
        isValid = false;
        $("#newAddressProof").after('<span class="error-message" style="color:red;">Beneficiary Address Proof is required.</span>');
    }

    // If any field is invalid, stop the submission
    if (!isValid) {
        return; // Stop further execution
    }

    // If all fields are valid, proceed with form submission
    var formData = new FormData();
    formData.append("Id", beneficiaryId);
    formData.append("BeneficiaryName", beneficiaryName);
    formData.append("AccountNumber", accountNumber);
    formData.append("BankIFSC", bankIFSC);
    formData.append("newIdProof", idProofFile);
    formData.append("newAddressProof", addressProofFile);

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