$(document).ready(function () {
    // Sidebar toggler
    $('#sidebarMenuOpen').click(function () {
        $('#sidebarMenu').addClass('open');
    });
    $('#sidebarMenuClose').click(function () {
        $('#sidebarMenu').removeClass('open');
    });

    // Back to top button
    var backToTopBtn = $('.back-to-top');

    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            backToTopBtn.fadeIn();
        } else {
            backToTopBtn.fadeOut();
        }
    });

    backToTopBtn.click(function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
        return false;
    });

    // Custom dropdown select (if applicable)
    $('.selectBtn').click(function () {
        $(this).next('.selectDropdown').toggleClass('show');
    });

    $('.select .option').click(function () {
        var value = $(this).text();
        $(this).parents('.select').find('.selectBtn').text(value);
        $(this).parents('.selectDropdown').removeClass('show');
    });

    // Toggle theme (dark/light mode)
    $('#changeTheme').change(function () {
        if (this.checked) {
            $('html').attr('class', 'dark');
        } else {
            $('html').attr('class', 'light');
        }
    });

    // Close dropdowns if clicked outside
    $(document).click(function (e) {
        if (!$(e.target).closest('.select').length) {
            $('.selectDropdown').removeClass('show');
        }
    });
});
