window.scrollToEnd = (el) => {
    if (el) {
        requestAnimationFrame(() => {
            el.scrollTop = el.scrollHeight;
        });
    }
};