let tg = window.Telegram?.WebApp;

if (tg) {
    tg.expand();

    let form = document.getElementById("truckbotForm");

    form.addEventListener("submit", (event) => {
        event.preventDefault();

        let callBackMethod = document.getElementById("callBackMethod").value;
        let userID = document.getElementById("userID").value;
        let firstName = document.getElementById("firstName").value;
        let secondName = document.getElementById("secondName").value;
        let lastName = document.getElementById("lastName").value;
        let phone = document.getElementById("phone").value;
        let driversLicense = document.getElementById("driversLicense").value;
        let isAdmin = document.getElementById("isAdmin").value;

        let messageDataInfoType = {
            callBackMethod: callBackMethod,
        }

        let vpoForm = {
            userID: userID,
            firstName: firstName,
            secondName: secondName,
            lastName: lastName,
            phone: phone,
            driversLicense: driversLicense,
            isAdmin: isAdmin,

        };

        let jsonArray = [messageDataInfoType, vpoForm];
        let jsonString = JSON.stringify(jsonArray);

        tg.sendData(jsonString);

        form.reset();

        tg.close();
    });
}