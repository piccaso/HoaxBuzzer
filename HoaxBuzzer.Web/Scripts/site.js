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

    function launchIntoFullscreen(element) {
        if (element.requestFullscreen) {
            element.requestFullscreen();
        } else if (element.mozRequestFullScreen) {
            element.mozRequestFullScreen();
        } else if (element.webkitRequestFullscreen) {
            element.webkitRequestFullscreen();
        } else if (element.msRequestFullscreen) {
            element.msRequestFullscreen();
        }
    }

    $('.fullscreen').on("click", function() {
        $(this).hide();
        launchIntoFullscreen(document.documentElement);
    });

    function setupCommandChannel() {
        var topic = window.channels.Command;
        var websocket = window.channels.Websocket;
        var client = mqtt.connect(websocket);
        var resetCommand = "$reset$";

        client.on("message", function(t, p) {
            if (topic === t) {
                var strPayload = p.toString();
                if (strPayload === resetCommand) {
                    window.location.reload(true);
                }
            }
        });
        client.subscribe(topic);
        $('.reset-command').on("click", function() {
            client.publish(topic, resetCommand);
        });
    }
    setupCommandChannel();
});