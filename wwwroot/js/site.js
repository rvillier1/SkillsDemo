var SortOrder = 'LastName'
var SortType = 'Asc'
var MenuItemClicked = 'About'

$(document).ready(function () {    
    HideLoading()
    $("a[data-menu-click]").click(function () {
        var linkName = this.innerText;
        MainMenuItemClick(linkName);        
    });
    
});

function MainMenuItemClick(linkName) {
    ShowLoading();
    ShowLoading();
    MenuItemClicked = linkName
    $("#LoadPanel").empty();    
    if (linkName === "About")        
        AboutPage(url)
    else if (linkName === "Database I/O") 
        DatabaseIOPage()
    if (linkName === "API I/O") 
        APIExamplePage()
}

function AboutPage() {
    url = BuildUrl("Home/index")
    $("#LoadPanel").load(url, null, function (response) {
    });
}

function DatabaseIOPage() {
    
    url = BuildUrl("Home/DatabaseIO") + "?SortOrder=" + SortOrder + "&SortType=" + SortType
    $("#LoadPanel").load(url, null, function (response) {
        $(".IOEdit").unbind();
        $(".IOEdit").bind("click", DatabaseIOEditClick);
        $(".IODelete").unbind();
        $(".IODelete").bind("click", DatabaseIODelete); 
        $("#AddNewEmployee").unbind();
        $("#AddNewEmployee").bind("click", AddNewEmployee); 
        

        HideLoading()
    });
}

function APIExamplePage() {
    url = BuildUrl("API/APIIO") + "?SortOrder=" + SortOrder + "&SortType=" + SortType + "&EmployeeID=0"
    $("#LoadPanel").load(url, null, function (response) {
        $(".IOEdit").unbind();
        $(".IOEdit").bind("click", DatabaseIOEditClick);
        $(".IODelete").unbind();
        $(".IODelete").bind("click", DatabaseIODelete);
        $("#AddNewEmployee").unbind();
        $("#AddNewEmployee").bind("click", AddNewEmployee);
        HideLoading();
    });
}

function DatabaseIOEditClick() {
    id = this.id;
    DatabaseIOEdit(id)    
}

function DatabaseIOEdit(id) {    
    if (MenuItemClicked === "Database I/O") 
        url = BuildUrl("Home/EditDBIO") + "?EmployeeID=" + id    
    else if (MenuItemClicked === "API I/O") 
        url = BuildUrl("API/EditAPIIO") + "?EmployeeID=" + id
    
    
    $("#LoadPanel").load(url, null, function (response) {
        $(".numbersOnly").unbind();
        $(".numbersOnly").bind("blur", numbersOnly);
        $(".numbersOnly").bind("focus", numbersOnly);
        $(".phone").bind("blur", FormatPhone);
        $('#FormEmployeeUpdates').submit(function (e) {
            e.preventDefault();
            EmployeeUpdates();
        });
    });
}

function DatabaseIODelete() {

    let text = "Are You sure you want to delete?";
    if (confirm(text) !== true) {
        return;
    }
    id = this.id;
    if (MenuItemClicked === "Database I/O")
        url = BuildUrl("Home/EmployeeDelete") + "?EmployeeID=" + id    
    else if (MenuItemClicked === "API I/O")
        url = BuildUrl("API/EmployeeDelete") + "?EmployeeID=" + id    
        
    $.ajax({
        url: url,
        type: "POST",
        dataType: 'json',        
        success: function (value) {
            HideLoading();
            if (MenuItemClicked === "Database I/O")
                DatabaseIOPage();
            else if (MenuItemClicked === "API I/O")
                APIExamplePage();
            
        },
        error: function (results) {
            HideLoading();
            alert("Failed the Update.");
        }
    });
}

function AddNewEmployee() {
    DatabaseIOEdit(0)
}

function EmployeeUpdates() {

    if ($("#FirstName").val().length === 0 ||
        $("#LastName").val().length === 0 ||
        $("#HireDate").val().length === 0 ||
        $("#BirthDate").val().length === 0) {
        alert("All Fileds with a red * are required")
        return false;
    }
    ShowLoading();
    ShowLoading();
    if (MenuItemClicked === "Database I/O")
        url = BuildUrl("Home/EmployeeUpdates")
    else if (MenuItemClicked === "API I/O")
        url = BuildUrl("API/EmployeeUpdates")



    var valdata = $("#FormEmployeeUpdates").serialize();

    $.ajax({
        url: url,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: valdata,
        success: function (value) {
            HideLoading();
            DatabaseIOPage()           
        },
        error: function (results) {
            HideLoading();
            alert("Failed the Update.");
        }
    });   
}

function numbersOnly() {
    if (parseInt(this.value) == 0 || this.value.length == 0)
        this.value = '';
    else {
        this.value = this.value.replace(/[^0-9\.]/g, '');
        this.value = parseInt(this.value)
    }
}

function FormatPhone() {
    var number = $(this).val().replace(/[^\d]/g, '')
    if (number.length === 10) {
        number = number.replace(/(\d{3})(\d{3})(\d{4})/, "($1) $2-$3");
        $(this).css("color", "black");
    }
    else if(number.length !== 0)
    {
        $(this).css("color", "red");
        $(this).focus()
    }
    $(this).val(number)
}

function sortTable(n) {

    if (SortOrder !== n)
        SortType = 'Asc'
    else {
        if (SortType === 'Asc')
            SortType = 'Desc'
        else
            SortType = 'Asc'
    }
    SortOrder = n

    if (MenuItemClicked === "Database I/O")
        DatabaseIOPage()
    else if (MenuItemClicked === "API I/O")
        APIExamplePage();
    
}

function BuildUrl(text) {
    urlHome = document.location.origin
    var Lastchar = urlHome.slice(-1);
    if (Lastchar != '/')
        urlHome = urlHome + '/' + text;
    else
        urlHome = urlHome + text;
    return urlHome;
}

function HideLoading() {
    //$.unblockUI();
    $('.shc-loading-img').addClass("HideLoding")
}

function ShowLoading() {
    //$.blockUI();
    $('.shc-loading-img').removeClass("HideLoding")
}