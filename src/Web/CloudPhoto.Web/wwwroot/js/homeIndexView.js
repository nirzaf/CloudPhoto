var stickyNavBarHeight;
var start = false;
$(document).ready(function () {
    var element = document.getElementById("myDivHeader");
    element.classList.add("myHeader");
    stickyNavBarHeight = element.offsetHeight;
    $('nav').removeClass('navbar-light');
    $('nav').removeClass('my-nav-color');
});
$(window).on("scroll", function () {
    if ($(window).scrollTop()) {
        $('nav').addClass('fixed-top');
    }
    else {
        $('nav').removeClass('fixed-top');
        $('nav').removeClass('navbar-light');
        $('nav').removeClass('my-nav-color');
    }
    if (window.pageYOffset >= stickyNavBarHeight) {
        $('nav').addClass('navbar-light');
        $('nav').addClass('my-nav-color');
    }
});
var HomeIndexViewHelper = /** @class */ (function () {
    function HomeIndexViewHelper(imagePerPage) {
        this.cookieHelper = new mainCookieHelper();
        this.cookieHelper.initSearchData(imagePerPage);
    }
    HomeIndexViewHelper.prototype.clickToCategory = function (categoryId) {
        this.cookieHelper.clearSearchData();
        this.cookieHelper.mainSearchData.selectCategory.push(categoryId);
        this.cookieHelper.saveSearchData();
        window.location.href = "/images/index";
    };
    return HomeIndexViewHelper;
}());
//# sourceMappingURL=homeIndexView.js.map