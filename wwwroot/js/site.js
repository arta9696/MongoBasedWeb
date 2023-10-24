// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function AddToList() {

    // Create an Option object        

    var opt = document.createElement("option");

    // Add an Option object to List Box

    document.getElementById("Friends").options.add(opt);
    opt.text = document.getElementById("AddFriends").value;
    opt.value = document.getElementById("AddFriends").value;
    document.getElementById("Friends").options.add(opt);

    return false;
}