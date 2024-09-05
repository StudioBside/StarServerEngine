(function () {
    // 현재 페이지 URL에서 일감 번호 추출
    const issueNumber = window.location.pathname.split('/').pop();

    // 일감 제목 추출
    const issueSubjectElement = document.querySelector('.subject');
    if (!issueSubjectElement) {
        return;
    }

    const issueTitleElement = issueSubjectElement.querySelector('h3');
    if (!issueTitleElement) {
        return; // 페이지에 일감 제목이 없으면 실행 중단
    }

    // 복사 버튼 생성
    const copyButton = document.createElement("button");
    copyButton.innerText = "P4 서밋 헤더 복사";
    issueTitleElement.parentElement.appendChild(copyButton);
    copyButton.addEventListener("click", function () {

        const issueTitle = issueTitleElement.innerText.trim();

        // 저장된 사용자 이름 가져오기
        var username = '';
        chrome.storage.sync.get('username', (data) => {
            if (!data.username) {
                username = prompt('사용자 이름을 입력해주세요.', '');
                if (username) {
                    chrome.storage.sync.set({ username: username }, () => {
                        console.log('사용자 이름이 저장되었습니다.');
                    });
                }
                else {
                    return;
                }
            }
            else
            {
                username = data.username;
            }

            const formattedText = `[${username}#${issueNumber}] ${issueTitle}\n- `;

            const notificationOption = {
                type: 'basic',
                title: '클립보드 복사 완료',
                iconUrl: 'icon.png',
                message: formattedText,
            }

            // 클립보드에 텍스트 복사
            if (navigator.clipboard !== undefined) {
                // 클립보드 API 사용가능한 경우 (https사용, 브라우저 지원여부 등등)
                navigator.clipboard.writeText(formattedText).then(() => {
                    chrome.runtime.sendMessage('', {
                        type: 'notification',
                        options: notificationOption,
                    });
                }).catch(err => {
                    console.error('Failed to copy text: ', err);
                });
            }
            else {
                // 클립보드 API 사용 불가능한 경우 deprecated된 execCommand 사용
                const textArea = document.createElement('textarea');
                textArea.value = formattedText;
                document.body.appendChild(textArea);
                textArea.select();
                document.execCommand('copy');
                document.body.removeChild(textArea);
                chrome.runtime.sendMessage('', {
                    type: 'notification',
                    options: notificationOption,
                });
            }
        });
    });
})();
