var IMainSearchData = /** @class */ (function () {
    function IMainSearchData() {
    }
    return IMainSearchData;
}());
var mainCookieHelper = /** @class */ (function () {
    function mainCookieHelper(imagePerPage) {
        this.mainCookieName = "searchData";
        this.mainSearchData = new IMainSearchData();
        this.mainSearchData.pageSize = imagePerPage;
        this.mainSearchData.pageIndex = 0;
        this.mainSearchData.currentSelectImage = 0;
        this.mainSearchData.selectCategory = new Array();
        this.mainSearchData.searchText = "";
    }
    mainCookieHelper.prototype.readSearchData = function () {
        var txtSearchData = $.cookie(this.mainCookieName);
        if (txtSearchData) {
            this.mainSearchData = JSON.parse(txtSearchData);
            return this.mainSearchData;
        }
        else {
            return this.mainSearchData;
        }
    };
    mainCookieHelper.prototype.saveSearchData = function () {
        var txtSearchData = JSON.stringify(this.mainSearchData);
        $.cookie(this.mainCookieName, txtSearchData, { path: "/" });
    };
    mainCookieHelper.prototype.clearSearchData = function () {
        $.cookie(this.mainCookieName, "", { path: "/" });
        this.mainSearchData.selectCategory = new Array();
        this.mainSearchData.searchText = "";
    };
    return mainCookieHelper;
}());
//# sourceMappingURL=mainSearchData.js.map