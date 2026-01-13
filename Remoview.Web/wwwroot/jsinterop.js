// wwwroot/jsinterop.js

window.localStorageFunctions = {
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    getItem: function (key) {
        return localStorage.getItem(key);
    },
    removeItem: function (key) {
        localStorage.removeItem(key);
    },

    toggleTheme: function () {
        var currentTheme = localStorage.getItem('theme');
        var newTheme = (currentTheme === 'light-mode') ? 'dark-mode' : 'light-mode';

        localStorage.setItem('theme', newTheme);

        // body class'ını komple ezmek yerine temiz uygulayalım
        document.body.classList.remove('light-mode', 'dark-mode');
        document.body.classList.add(newTheme);

        return newTheme;
    },

    applyInitialTheme: function () {
        var savedTheme = localStorage.getItem('theme');

        if (savedTheme !== 'light-mode' && savedTheme !== 'dark-mode') {
            savedTheme = 'dark-mode';
            localStorage.setItem('theme', savedTheme);
        }

        document.body.classList.remove('light-mode', 'dark-mode');
        document.body.classList.add(savedTheme);
    }
};

// ✅ Carousel scroll fonksiyonu GLOBAL olmalı (localStorageFunctions içine gömülmez!)
window.rmScrollCarousel = function (id, dir) {
    const el = document.getElementById(id);
    if (!el) return;

    const amount = Math.round(el.clientWidth * 0.85);
    el.scrollBy({ left: dir * amount, behavior: "smooth" });
};
