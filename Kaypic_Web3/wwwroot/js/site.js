//***********Partie chabot, compte Parent
document.addEventListener("DOMContentLoaded", function () {
    const chatbotBtn = document.getElementById("chatbot-btn");
    const chatbotWindow = document.getElementById("chatbot-window");
    const chatBody = document.getElementById("chat-body");

    const qaPairs = {
        "Quels sont les noms des coachs disponibles?": async () => {
            try {
                const response = await fetch("/Home/GetCoachs");
                const data = await response.json();
                return "Nos coachs disponibles sont : " + data.join(", ");
            } catch (error) {
                return "Désolé, je n’ai pas pu récupérer la liste des coachs.";
            }
        },
        "Quand est mon prochain événement?": async () => {
            try {
                const response = await fetch("/Calendriers/GetNextEvent");
                const data = await response.json();
                return data;
            } catch (e) {
                return "Désolé, je n'ai pas pu récupérer vos événements.";
            }
        },
        "Comment puis-je créer un nouveau groupe?": "Allez dans l'onglet messages et appuyez sur le signe '+' à côté des conversations",
        "Comment puis-je contacter le support?": "Vous pouvez nous rejoindre à support@example.com."
    };

    //Afficher la fenetre du chatbot
    chatbotBtn.addEventListener("click", () => {
        chatbotWindow.classList.toggle("hidden");
    });

    //Clicque sur une question prédéfinie
    document.querySelectorAll(".question").forEach(btn => {
        btn.addEventListener("click", async () => {
            const question = btn.textContent;
            const answerFunc = qaPairs[question];

            appendMessage("user-msg", question);

            let answer;
            if (typeof answerFunc === "function") {
                answer = await answerFunc();
            } else {
                answer = answerFunc;
            }

            setTimeout(() => appendMessage("bot-msg", answer), 500);
        });
    });

    function appendMessage(className, text) {
        const msg = document.createElement("p");
        msg.className = className;
        msg.textContent = text;
        chatBody.appendChild(msg);
        chatBody.scrollTop = chatBody.scrollHeight;
    }
});


//**************Partie pour changer le MDP
function showCustomAlert(message, title = "Alert!") {
    const alertBox = document.getElementById("customAlert");
    document.getElementById("customAlertTitle").innerText = title;
    document.getElementById("customAlertMessage").innerText = message;
    alertBox.style.display = "flex";
}

function closeCustomAlert() {
    document.getElementById("customAlert").style.display = "none";
}

function openPasswordModal() {
    document.getElementById("passwordModal").style.display = "flex";
}
function closePasswordModal() {
    document.getElementById("passwordModal").style.display = "none";
}

document.getElementById("passwordForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const oldPassword = document.getElementById("oldPassword").value;
    const newPassword = document.getElementById("newPassword").value;
    const confirmPassword = document.getElementById("confirmPassword").value;

    if (newPassword !== confirmPassword) {
        showCustomAlert("Les mots de passe ne correspondent pas.");
        return;
    }

    try {
        const response = await fetch('/Account/ChangePassword', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ oldMDP: oldPassword, newMDP: newPassword })
        });

        const result = await response.json();

        if (response.ok) {
            showCustomAlert(result.message);
            closePasswordModal();
        } else {
            showCustomAlert(result.message);
        }
    } catch (err) {
        console.error(err);
        showCustomAlert("Une erreur est survenue..");
    }
});