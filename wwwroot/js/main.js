(function ($) {
    "use strict";

    $(window).on('load', function () {
        setTimeout(function () {
            $('#spinner').addClass('hide');
        }, 100); 
    });

    new WOW().init();

})(jQuery);