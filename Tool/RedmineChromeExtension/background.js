chrome.tabs.onActivated.addListener(activeInfo => {
  chrome.tabs.get(activeInfo.tabId, (tab) => {
    if (tab.url && tab.url.includes('redmine.org/issues/')) {
      chrome.scripting.executeScript({
        target: { tabId: tab.id },
        files: ['content.js']
      });
    }
  });
});

chrome.runtime.onMessage.addListener(data => {
  const notificationId = 'p4summit-copy-notification';
  if (data.type === 'notification') {
    // notification API는 background에서만 사용 가능
    chrome.notifications.create(notificationId, data.options);
  }
});
