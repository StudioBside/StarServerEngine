document.getElementById('save').addEventListener('click', () => {
    const username = document.getElementById('username').value;
    if (username) {
      chrome.storage.sync.set({ username: username }, () => {
        alert('Username saved.');
      });
    }
  });
  