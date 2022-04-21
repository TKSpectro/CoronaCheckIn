function switchLanguage(dropdown) {
    const loc = dropdown.options[dropdown.selectedIndex].value;

    $.ajax({
        url: '/Home/SetLang',
        type: 'POST',
        data: {
            lang: loc,
            url: window.location.pathname,
        },
        success: function (result) {
            window.location.pathname = result;
            window.location.reload();
        },
    });
}
