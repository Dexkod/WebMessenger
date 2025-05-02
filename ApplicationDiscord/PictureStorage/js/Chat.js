export function scrollToBottom(){
    document.querySelectorAll('.chat-container').forEach(container => {
        container.scrollTop = container.scrollHeight;
    });
};