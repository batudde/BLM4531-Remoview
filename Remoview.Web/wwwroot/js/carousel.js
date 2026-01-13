window.rmCarousel = {
    scroll: function (id, dir) {
        const el = document.getElementById(id);
        if (!el) return;

        // 1 kart + gap kadar kaydır (responsive)
        const firstCard = el.querySelector(".film-card");
        const cardW = firstCard ? firstCard.getBoundingClientRect().width : 240;

        // gap'ı CSS'ten okuyalım (varsa)
        const styles = window.getComputedStyle(el);
        const gap = parseFloat(styles.columnGap || styles.gap || "0") || 12;

        const amount = (cardW + gap) * 2; // 2 kartlık kaydırma
        el.scrollBy({ left: dir * amount, behavior: "smooth" });
    }
};
