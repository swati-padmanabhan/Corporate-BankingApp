function loadEmployees() {
    $.ajax({
        url: "/Client/GetAllEmployees",
        type: "GET",
        success: function (data) {
            /* console.log(data);*/
            $("#employeeTable").empty();

            if (data.length > 0) {
                $.each(data, function (index, employee) {
                    var row = `<tr>
                    <td>${employee.FirstName}</td>
                    <td>${employee.LastName}</td>
                    <td>${employee.Email}</td>
                    <td>${employee.Phone}</td>  
                    <td>${employee.Designation}</td>

                    
                     <td>
                         <input type="checkbox" class="is-active-checkbox"
                                data-employeeid="${employee.Id}"
                                ${employee.IsActive ? "checked" : ""} />
                     </td>
                  <td>
                            <button onClick="editEmployee('${employee.Id}')" class="action-btn btn btn-secondary btn-sm" title="Edit">
                                <i class="bi bi-pencil-square"></i>
                            </button>
                        </td>

                    </tr>`;
                    $("#employeeTable").append(row);
                });

                // Add event listener for all checkboxes
                $(".is-active-checkbox").change(function () {
                    var employeeId = $(this).data("employeeid");
                    var isActive = $(this).is(":checked");

                    // Call the server to update the contact status
                    updateEmployeeStatus(employeeId, isActive);
                });
            }
            else {
                $("#employeeTable").append("<tr><td colspan='5'>No employees found.</td></tr>");
            }
        },
        error: function (err) {
            console.log("Error Retrieving Employees:", err);
        }
    });
}

function addNewEmployee() {
    var newEmployee = {
        FirstName: $("#firstName").val(),
        LastName: $("#lastName").val(),
        Email: $("#email").val(),
        Designation: $("#designation").val(),
        Phone: $("#phone").val()
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
    })
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
                    `Employee's status has been ${isActive ? "activated" : "deactivated"
                    }.`
                );
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
    $(`#${ type }Modal`).modal("show");
    $(`#${ type }ModalLabel`).text(header);
    $(`#${ type }ModalBody`).text(message);
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
        Designation: $("#newDesignation").val()
    };
    modifyRecord(data);
});