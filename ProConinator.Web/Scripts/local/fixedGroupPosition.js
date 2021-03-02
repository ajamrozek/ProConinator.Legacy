var origTop = null;
var origGroupLeft = null;
var origGroupsWidth = null;
var origGroupWidth = null;
var origGroupHeight = null;
var groups = null;
var group = null;

initGroupPosition = function () {
    if ($(".proConGroups").length > 0) {
        groups = $(".proConGroups");
        group = $(".groupContainer");
        //origTop = group.offset().top - $(window).scrollTop();
        origGroupLeft = group.offset().left + 4;
        origGroupWidth = group.width();
        origGroupsWidth = groups.width();
        origGroupHeight = group.height();
    }
};

function debounce(func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
};

$(window).scroll(
    function () {
        if (groups != null) {
            var origTop = (group.offset().top + group.position().top) - $(document).scrollTop();
            var difference = origTop - $(document).scrollTop();
            if (difference <= 0) {
                groups.addClass("fixedGroups");
                group.addClass("fixedGroup");
            } else {
                groups.removeClass("fixedGroups");
                group.removeClass("fixedGroup");
            }
        }
    });