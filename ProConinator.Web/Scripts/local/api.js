function login(url, viewModel) {
    $.post(url, viewModel, function (data) {
        alert('success');
    });
}