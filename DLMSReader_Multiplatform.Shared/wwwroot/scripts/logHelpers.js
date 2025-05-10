window.scrollToEnd = (el) => {
    if (el) {
        // vyčkáme, než Blazor/DOM vloží nové řádky,
        // jinak může scroll proběhnout „předčasně“
        requestAnimationFrame(() => {
            el.scrollTop = el.scrollHeight;
        });
    }
};