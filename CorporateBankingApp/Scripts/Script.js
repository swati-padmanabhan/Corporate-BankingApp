function openModal(title, imageUrl) {
    console.log("modal clicked");
    console.log(title, imageUrl);

    // Set the modal title
    $("#documentModalLabel").text(title);

    // Set the modal image source
    $("#modalImage").attr("src", imageUrl);

    // Show the modal
    $("#documentModal").modal("show");
}