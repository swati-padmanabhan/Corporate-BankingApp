let employeesData = [];
let currentEmployeePage = 1;
const employeePageSize = 10; // Number of items per page



// Function to load employees from the server
function loadEmployees() {
    $.ajax({
        url: "/Client/GetAllEmployees",
        type: "GET",
        success: function (data) {
            employeesData = data; // Store all employees data
            setupEmployeePagination(); // Setup pagination
            renderEmployeeTable(); // Render the first page
        },
        error: function (err) {
            console.log("Error Retrieving Employees:", err);
        }
    });
}

function renderEmployeeTable() {
    $("#employeeTable").empty();
    const start = (currentEmployeePage - 1) * employeePageSize;
    const end = start + employeePageSize;
    const paginatedData = employeesData.slice(start, end);

    if (paginatedData.length > 0) {
        $.each(paginatedData, function (index, employee) {
            const row = `<tr>
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

        // Attach event listeners for checkboxes and other actions
        attachEventListeners();
    } else {
        // Alert for no employees found
        $("#employeeTable").append(`<tr><td colspan='9' class="text-center">No employees found.</td></tr>`);
    }
}


function setupEmployeePagination() {
    const totalPages = Math.ceil(employeesData.length / employeePageSize);
    const paginationHtml = [];

    for (let i = 1; i <= totalPages; i++) {
        paginationHtml.push(`<li class="page-item ${i === currentEmployeePage ? 'active' : ''}">
            <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
        </li>`);
    }

    $("#employeePagination").html(paginationHtml.join(''));

}

function changePage(page) {
    currentEmployeePage = page;
    renderEmployeeTable(); // Render the new page
}

// Function to attach event listeners for checkboxes
function attachEventListeners() {
    $(".is-active-checkbox").change(function () {
        var employeeId = $(this).data("employeeid");
        var isActive = $(this).is(":checked");
        updateEmployeeStatus(employeeId, isActive);
    });

    $(".is-SalaryDisbursed-checkbox").change(function () {
        updateTotalSalary();
    });

    $("#selectAllSalaryDisbursement").change(function () {
        var isChecked = $(this).is(":checked");
        $(".is-SalaryDisbursed-checkbox").prop("checked", isChecked).trigger("change");
        if (!isChecked) {
            updateTotalSalary(); // Reset total salary when unchecked
        }
    });

    // Initialize total salary on page load
    updateTotalSalary();
}

// Function to update total salary based on selected employees
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

// Function to add a new employee
function addNewEmployee() {
    // Clear previous error messages
    $(".error-message").remove();

    // Validate fields
    var isValid = true;

    // Validate First Name
    var firstName = $("#firstName").val().trim();
    if (!firstName) {
        isValid = false;
        $("#firstName").after('<span class="error-message" style="color:red;">First Name is required.</span>');
    }

    // Validate Last Name
    var lastName = $("#lastName").val().trim();
    if (!lastName) {
        isValid = false;
        $("#lastName").after('<span class="error-message" style="color:red;">Last Name is required.</span>');
    }

    // Validate Email
    var email = $("#email").val().trim();
    if (!email) {
        isValid = false;
        $("#email").after('<span class="error-message" style="color:red;">Email is required.</span>');
    } else if (!validateEmail(email)) {
        isValid = false;
        $("#email").after('<span class="error-message" style="color:red;">Invalid email format.</span>');
    }

    // Validate Phone
    var phone = $("#phone").val().trim();
    var phonePattern = /^\d{10}$/;
    if (!phone) {
        $("#phone").after('<span class="error-message" style="color:red;">Phone number is required.</span>');
        isValid = false;
    } else if (!phonePattern.test(phone)) {
        $("#phone").after('<span class="error-message" style="color:red;">Phone number must be exactly 10 digits.</span>');
        isValid = false;
    }

    // Validate Designation
    var designation = $("#designation").val().trim();
    if (!designation) {
        isValid = false;
        $("#designation").after('<span class="error-message" style="color:red;">Designation is required.</span>');
    }

    // Validate Salary
    var salary = $("#salary").val().trim();
    if (!salary) {
        isValid = false;
        $("#salary").after('<span class="error-message" style="color:red;">Salary is required.</span>');
    } else if (isNaN(salary) || salary <= 0) {
        isValid = false;
        $("#salary").after('<span class="error-message" style="color:red;">Salary must be a positive number.</span>');
    }

    // If any field is invalid, stop the submission
    if (!isValid) {
        return; // Stop further execution
    }

    // If all fields are valid, proceed with the AJAX call
    var newEmployee = {
        FirstName: firstName,
        LastName: lastName,
        Email: email,
        Phone: phone,
        Designation: designation,
        Salary: salary,
    };

    $.ajax({
        url: "/Client/Add",
        type: "POST",
        data: newEmployee,
        success: function (response) {
            if (response.success) {
                alert("New Employee Added Successfully");
                loadEmployees();
                $("#newRecord").hide();
                $("#employeeList").show();
            } else {
                alert(response.message); // Handle validation errors from the server
            }
        },
        error: function (err) {
            alert("Error Adding New Employee");
            console.log(err);
        }
    });
}

// Email validation function
function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}


// Function to get an employee's details for editing
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

// Function to modify an employee's record
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
                $("#editRecord").hide();
                window.scrollTo(0, 0);
            } else {
                alert(response.message);
            }
        },
        error: function (err) {
            $("#employeeList").hide();
            alert("Error Editing Employee Record");
        }
    });
}

// Function to update an employee's active status
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
        }
    });
}

// Function to show modal messages
function showModal(type, header, message) {
    $(`#${type}Modal`).modal("show");
    $(`#${type}ModalLabel`).text(header);
    $(`#${type}ModalBody`).text(message);
}

// Event handler to add a new employee form
$("#btnAdd").click(function () {
    $("#employeeList").hide();
    $("#newRecord").show();
});

// Event handler to edit employee
function editEmployee(employeeId) {
    getEmployee(employeeId);
    $("#employeeList").hide();
    $("#editRecord").show();
}


// Event handler for save button on edit
$("#btnEdit").click(function (event) {
    event.preventDefault(); // Prevent form submission
        var data = {
            Id: $("#editEmployeeId").val(),
            FirstName: $("#newFirstName").val(),
            LastName: $("#newLastName").val(),
            Email: $("#newEmail").val(),
            Phone: $("#newPhone").val(),
            Designation: $("#newDesignation").val(),
            Salary: $("#newSalary").val()
        };
        modifyRecord(data); // Call the function to modify the record
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
