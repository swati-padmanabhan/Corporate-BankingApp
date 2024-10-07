var selectedIds = [];

function loadInboundBeneficiaries() {
    $.ajax({
        url: "/Client/GetAllInboundBeneficiaries",
        type: "GET",
        success: function (data) {
            $("#inboundBeneficiaryTblBody").empty();
            if (data.length <= 0) {
                alert("No inbound beneficiaries found.");
                window.history.back(); // Go back to the previous page
            } else {
                $.each(data, function (index, item) {
                    var statusBadgeClass = item.BeneficiaryStatus === 'PENDING' ? 'bg-warning' : 'bg-success';
                    var row = `<tr>
                        <td>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input inbound-checkbox" id="check-${item.Id}" data-beneficiaryid="${item.Id}" />
                                <label class="form-check-label" for="check-${item.Id}"></label>
                            </div>
                        </td>
                        <td>${item.CompanyName}</td>
                        <td>${item.AccountNumber}</td>
                        <td>${item.ClientIFSC}</td>
                        <td>
                            <span class="badge ${statusBadgeClass} rounded-pill d-inline">${item.BeneficiaryStatus}</span>
                        </td>
                    </tr>`;
                    $("#inboundBeneficiaryTblBody").append(row);
                });

                // Re-attach change event after populating the table
                attachCheckboxChangeEvents();
            }
        },
        error: function (err) {
            $("#inboundBeneficiaryTblBody").empty();
            alert("No data available");
        }
    });
}

// Attach change events to checkboxes
function attachCheckboxChangeEvents() {
    $("#inboundBeneficiaryTblBody").on("change", ".inbound-checkbox", function () {
        var inboundId = $(this).data('beneficiaryid');
        if ($(this).is(":checked")) {
            if (!selectedIds.includes(inboundId)) {
                selectedIds.push(inboundId); // Add to selectedIds if checked
            }
        } else {
            selectedIds = selectedIds.filter(id => id !== inboundId); // Remove from selectedIds if unchecked
        }
        console.log("Selected Inbounds: ", selectedIds);
    });

    // Handle select all checkbox
    $("#selectAllInbounds").off('change').on('change', function () {
        selectedIds = []; // Clear selected IDs before selecting all
        if ($(this).is(":checked")) {
            $(".inbound-checkbox").prop("checked", true).each(function () {
                var inboundId = $(this).data('beneficiaryid');
                selectedIds.push(inboundId); // Add all IDs to selectedIds
            });
        } else {
            $(".inbound-checkbox").prop("checked", false);
        }
        console.log("Selected Inbounds: ", selectedIds);
    });
}

// Approve selected disbursements
$('#addInboundSelected').click(function () {
    console.log(selectedIds); // Display selected IDs in console
    if (selectedIds.length === 0) {
        alert("Please select at least one beneficiary.");
        return;
    }

    $.ajax({
        type: "POST",
        url: "/Client/AddInboundBeneficiary",
        data: { beneficiaryIds: selectedIds },
        traditional: true, // This ensures the array is sent correctly
        success: function (response) {
            if (response.success) {
                alert(response.message);
                window.location.href = `/Client/ManageBeneficiaries`;
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            alert("An error occurred while adding the beneficiary: " + error);
        }
    });
});
