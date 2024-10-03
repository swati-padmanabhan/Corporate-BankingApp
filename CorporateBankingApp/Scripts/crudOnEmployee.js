function loadEmployees() {
    $.ajax({
        url: "/Client/GetAllEmployees",
        type: "GET",
        success: function (data) {
            $("#employeeTable").empty();

            if (data.length > 0) {
                $.each(data, function (index, employee) {
                    var row = `<tr>
                        <td>${employee.FirstName}</td>
                        <td>${employee.LastName}</td>
                        <td>${employee.Email}</td>
                        <td>${employee.Phone}</td>
                        <td>${employee.Designation}</td>
                        <td>${employee.Salary}</td>
                        <td>
                            <input type="checkbox" class="is-active-checkbox"
                                data-employeeid="${employee.Id}"
                                data-toggle="toggle" 
                                data-onstyle="outline-danger" 
                                data-offstyle="outline-warning"
                                ${employee.IsActive ? "checked" : ""} />
                        </td>
                        <td>
                            <input type="checkbox" class="is-SalaryDisbursed-checkbox"
                                data-employeeid="${employee.Id}"
                                data-salary="${employee.Salary}"
                                ${employee.SalaryDisburseSelect ? "checked" : ""} 
                                style="${employee.IsActive ? '' : 'display:none'}" />
                        </td>
                        <td>
                            <button onClick="editEmployee('${employee.Id}')" class="action-btn btn btn-secondary btn-sm" title="Edit" style="${employee.IsActive ? '' : 'display:none'}">
                                <i class="bi bi-pencil-square"></i>
                            </button>
                        </td>
                    </tr>`;
                    $("#employeeTable").append(row);
                });

                // Event listener for changing the active status
                $(".is-active-checkbox").change(function () {
                    var employeeId = $(this).data("employeeid");
                    var isActive = $(this).is(":checked");
                    updateEmployeeStatus(employeeId, isActive);
                });

                // Event listener for salary disbursement checkbox
                $(".is-SalaryDisbursed-checkbox").change(function () {
                    updateTotalSalary();
                });

                // Select all salary disbursement checkboxes
                $("#selectAllSalaryDisbursement").change(function () {
                    var isChecked = $(this).is(":checked");
                    $(".is-SalaryDisbursed-checkbox").prop("checked", isChecked).trigger("change");
                    if (!isChecked) {
                        updateTotalSalary(); // Reset total salary when unchecked
                    }
                });

                // Initialize total salary on page load
                updateTotalSalary();
            } else {
                // Create and append alert for no employees found
                var alertMessage = `<div class="alert alert-warning text-center" role="alert">
                                      No employees found.
                                    </div>`;
                $("#employeeTable").append(`<tr><td colspan='9'>${alertMessage}</td></tr>`);
            }
        },
        error: function (err) {
            console.log("Error Retrieving Employees:", err);
        }
    });
}
function updateTotalSalary() {
    let totalSalary = 0;
    $(".is-SalaryDisbursed-checkbox:checked").each(function () {
        let employeeId = $(this).data("employeeid");
        let salary = parseFloat($(this).data("salary"));

        // Check if the employee is active
        let isActive = $(".is-active-checkbox[data-employeeid='" + employeeId + "']").is(":checked");

        // Only include salary if the employee is active
        if (isActive) {
            totalSalary += salary;
        }
    });
    $("#salaryAmountInput").val(totalSalary.toFixed(2)); // Update the total salary
}

// Function to handle salary disbursement
$('#disburseSalary').click(function () {
    var employeeIds = [];

    // Loop through all the checkboxes that are checked for salary disbursement
    $('.is-SalaryDisbursed-checkbox:checked').each(function () {
        employeeIds.push($(this).data('employeeid'));
    });
    var isBatch = employeeIds.length > 1;

    if (employeeIds.length > 0) {
        $.ajax({
            type: 'POST',
            url: '/Client/ProcessSalaryDisbursements',
            data: { employeeIds: employeeIds, isBatch: isBatch },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    loadEmployees(); // Reload the employee list after disbursement
                    $('#selectAllSalaryDisbursement').prop('checked', false);
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        alert("No employees selected for salary disbursement.");
    }
});

function addNewEmployee() {
    var newEmployee = {
        FirstName: $("#firstName").val(),
        LastName: $("#lastName").val(),
        Email: $("#email").val(),
        Phone: $("#phone").val(),
        Designation: $("#designation").val(),
        Salary: $("#salary").val(),
    };

    $.ajax({
        url: "/Client/Add",
        type: "POST",
        data: newEmployee,
        success: function (response) {
            alert("New Employee Added Successfully");
            loadEmployees();
            $("#newRecord").hide();
            $("#employeeList").show();
        },
        error: function (err) {
            alert("Error Adding New Employee");
            console.log(err);
        }
    });
}

function getEmployee(employeeId) {
    $.ajax({
        url: "/Client/GetEmployeeById",
        type: "GET",
        data: { id: employeeId },
        success: function (response) {
            if (response.success) {
                $("#editEmployeeId").val(response.employee.Id);
                $("#newFirstName").val(response.employee.FirstName);
                $("#newLastName").val(response.employee.LastName);
                $("#newEmail").val(response.employee.Email);
                $("#newPhone").val(response.employee.Phone);
                $("#newDesignation").val(response.employee.Designation);
                $("#newSalary").val(response.employee.Salary);
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            alert("No Such Data Found");
        }
    });
}

function modifyRecord(modifiedEmployee) {
    $.ajax({
        url: "/Client/Edit",
        type: "POST",
        data: modifiedEmployee,
        success: function (response) {
            if (response.success) {
                alert("Employee Edited Successfully");
                loadEmployees();
                $("#employeeList").show();
                $("#editEmployee").hide();
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            alert("Error Editing Employee Record");
        }
    });
}

function updateEmployeeStatus(employeeId, isActive) {
    $.ajax({
        url: "/Client/UpdateEmployeeStatus",
        type: "POST",
        data: { Id: employeeId, isActive: isActive },
        success: function (response) {
            if (response.success) {
                showModal(
                    "success",
                    "Employee Status Updated",
                    `Employee's status has been ${isActive ? "activated" : "deactivated"}.`
                );
                loadEmployees(); // Reload the employee list to reflect status change
            } else {
                showModal(
                    "error",
                    "Error updating employee status",
                    "Something went wrong."
                );
            }
        },
        error: function (err) {
            console.log("Error Updating Employee Status: ", err);
        },
    });
}

function showModal(type, header, message) {
    $(`#${type}Modal`).modal("show");
    $(`#${type}ModalLabel`).text(header);
    $(`#${type}ModalBody`).text(message);
}

$("#btnAdd").click(function () {
    $("#employeeList").hide();
    $("#newRecord").show();
});

function editEmployee(employeeId) {
    getEmployee(employeeId);
    $("#employeeList").hide();
    $("#editRecord").show();
}

$("#btnEdit").click(() => {
    var data = {
        Id: $("#editEmployeeId").val(),
        FirstName: $("#newFirstName").val(),
        LastName: $("#newLastName").val(),
        Email: $("#newEmail").val(),
        Phone: $("#newPhone").val(),
        Designation: $("#newDesignation").val(),
        Salary: $("#newSalary").val()
    };
    modifyRecord(data);
});

// CSV file upload
$("#uploadCSVForm").on("submit", function (event) {
    event.preventDefault(); // Prevent the form from submitting the default way
    var formData = new FormData(this); // Create FormData object with form data

    // Make an AJAX call to upload the CSV
    $.ajax({
        url: "/Client/UploadCsv",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            $('#uploadCSVModal').modal('hide'); // Hide the modal on success
            loadEmployees(); // Reload the employee list
            alert('CSV uploaded successfully!');
        },
        error: function (error) {
            alert('Error uploading CSV. Please try again.');
            console.log("Error uploading CSV: ", error);
        }
    });
});

// Optional: To reset the form when the modal is closed
$('#uploadCSVModal').on('hidden.bs.modal', function () {
    $(this).find('form')[0].reset(); // Reset the form
});

$(document).ready(() => {
    loadEmployees();
});
