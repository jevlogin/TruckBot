document.getElementById("userType").addEventListener("change", function () {
    var userType = document.getElementById("userType").value;
    if (userType === "driver") {
        document.getElementById("driverFields").style.display = "block";
        document.getElementById("adminFields").style.display = "none";
    } else if (userType === "admin") {
        document.getElementById("driverFields").style.display = "none";
        document.getElementById("adminFields").style.display = "block";
    } else {
        document.getElementById("driverFields").style.display = "none";
        document.getElementById("adminFields").style.display = "none";
    }
});
