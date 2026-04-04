// Global cookie helpers required by CookieService via JS interop.
// Included as a non-module so functions are available on window.
window.setCookie = function (key, value, days) {
  var expires = "";
  if (days && !isNaN(days)) {
    var date = new Date();
    date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
    expires = "; expires=" + date.toUTCString();
  }
  document.cookie = encodeURIComponent(key) + "=" + encodeURIComponent(value) + expires + "; path=/";
};

window.getCookie = function (key) {
  var name = encodeURIComponent(key) + "=";
  var ca = document.cookie.split(';');
  for (var i = 0; i < ca.length; i++) {
    var c = ca[i].trim();
    if (c.indexOf(name) === 0) return decodeURIComponent(c.substring(name.length, c.length));
  }
  return null;
};

window.deleteCookie = function (key) {
  // Set expiry in the past to delete
  document.cookie = encodeURIComponent(key) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
};