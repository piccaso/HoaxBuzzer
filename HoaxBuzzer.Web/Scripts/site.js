jQuery.fn.StartCssAnimation = function(className) {
    var $self = $(this);
    $self.removeClass(className);
    setTimeout(function () {
        $self.addClass(className);
        $self.one("webkitAnimationEnd oanimationend msAnimationEnd animationend", function () {
            $self.removeClass(className);
        });
    }, 1);
};