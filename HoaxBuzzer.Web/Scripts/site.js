jQuery.fn.StartCssAnimation = function (className) {
    "use strict";
    var $self = $(this);
    $self.removeClass(className);
    setTimeout(function () {
        $self.addClass(className);
        $self.one("webkitAnimationEnd oanimationend msAnimationEnd animationend", function () {
            $self.removeClass(className);
        });
    }, 1);
};

jQuery(document).ready(function ($) {
    "use strict";
    var ajaxHeartBeat = config.ajaxHeartBeat;

    function beat() {
        $.ajax({
            url: ajaxHeartBeat,
            error : function() {
                window.location.reload(true);
            }
        });

    }
    function rnd(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    setInterval(beat, rnd(20000,30000));
});