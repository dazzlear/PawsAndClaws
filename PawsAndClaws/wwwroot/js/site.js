// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
  // Dropdown Toggle Logic
  $(document).on("click", ".dropdown-toggle", function (e) {
    e.stopPropagation();
    const menu = $(this).next(".dropdown-menu");

    // Close other open menus
    $(".dropdown-menu").not(menu).addClass("hidden");

    // Toggle current menu
    menu.toggleClass("hidden");
  });

  // Close dropdowns when clicking outside
  $(document).on("click", function () {
    $(".dropdown-menu").addClass("hidden");
  });

  // Prevent menu closure when clicking inside the menu
  $(document).on("click", ".dropdown-menu", function (e) {
    e.stopPropagation();
  });
});
