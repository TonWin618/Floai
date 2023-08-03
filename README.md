# Floai
Chat with AI through a transparent floating window.
![image](images/showcase.png)


## Features

- **Transparent background & Floating button**: It can assist you in your work without interrupting your workflow.

- **Word-by-word display**: The replies are displayed word-by-word, similar to how it's shown on the ChatGPT website.

- **Markdown syntax & code highlighting**: Make text reading easier

- **Tray icon**: The application won't occupy your taskbar when minimized, as it is represented by a tray icon.

- **Save message records in JSON format**: The application can save the conversation history in JSON format, making it easier for you to review, modify, and perform other operations.

- **Custom API**: You can access any API by customizing the HTTP requests.

## Releases
| Version | Release date | Download                                                     |
| ------- | ------------ | ------------------------------------------------------------ |
| v1.0.0  | May 13,2023  | [Floai v1.0.0](https://github.com/TonWin618/Floai/releases/tag/v1.0.0) |
| v1.1.0  | Jun 7,2023  | [Floai v1.1.0](https://github.com/TonWin618/Floai/releases/tag/v1.1.0) |
| v1.2.0  | Jun 10,2023  | [Floai v1.2.0](https://github.com/TonWin618/Floai/releases/tag/v1.2.0) |
| v1.3.0  | Jun 18,2023  | [Floai v1.3.0](https://github.com/TonWin618/Floai/releases/tag/v1.3.0) |
| v1.3.1  | Jul 6,2023  | [Floai v1.3.1](https://github.com/TonWin618/Floai/releases/tag/v1.3.1) |

## How to Use
1. Download the compressed package for the corresponding platform and extract it.  
Here is the latest version: [Floai-releases-latest](https://github.com/TonWin618/Floai/releases/latest)

2. Launch the application. You will see a floating button, click on it to open the chat interface.  
![image](images/step2.png)

3. Before chatting, you need to configure the ApiKey. Right-click on the icon in the taskbar and click on the "Settings" button to open the settings interface.  
![image](images/Step3.png)

4. Enter your API Key in the text box and click the "Add" button.  
[How to get an OpenAI API Key?](https://platform.openai.com/account/api-keys)  
![image](images/step4.png)

5. Close the settings interface and return to the chat interface. Start having conversations with ChatGPT.  
![image](images/step5.png)

> Tip: In the chat interface, press `ESC` to close the chat window, and press `Ctrl+Enter` to send a message.

## Custom API
### Aliyun TongYiQwen
```json
{
    "url": "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation",
    "headers": {
        "Authorization": "Bearer sk-"
    },
    "params": {},
    "historyFormat": "{\"${user_sender}\": \"${user_content}\", \"${ai_sender}\": \"${ai_content}\"}",
    "userRoleName": "user",
    "aiRoleName": "bot",
    "body": "{\"model\": \"qwen-v1\", \"input\": {\"prompt\":\"${prompt}\", \"history\":[${history}]}, \"parameters\": {}}",
    "contentPath": "output/text"
}
```

### Third-Party OpenAI API
```json
{
    "url": "https://example.com",
    "headers": {
        "Authorization": "Bearer sk-"
    },
    "params": {},
    "historyTemplate": "{\"role\": \"${sender}\", \"content\": \"${content}\"}",
    "userRoleName": "user",
    "aiRoleName": "assistant",
    "body": "{\"model\": \"gpt-3.5-turbo\", \"messages\": [${history},{\"role\": \"user\", \"content\": \"${prompt}\"}], \"temperature\": 0.7}",
    "contentPath": "choices/0/message/content"
}
```

### How to Define Your Own API Client: 

1. Open the settings interface.  

2. Switch to **Http **ApiClient. 

3. Fill in the configuration information.  

   | Key             | Description                                                  |
   | --------------- | ------------------------------------------------------------ |
   | url             | URL                                                          |
   | params          | URL Parameters                                               |
   | headers         | Request Headers                                              |
   | body            | Request Body                                                 |
   | contentPath     | The path from the request body to the returned text message. |
   | historyTemplate | Template for message logs, connected with a comma by the program, then replaces the `${history}` placeholder in the body. |
   | userRoleName    | Used to replace the `${sender}` or `${user_sender}` placeholders. |
   | aiRoleName      | Used to replace the `${sender}` or `${ai_sender}` placeholders. |

   

4. **Restart** the application.  

You can use `${}` to represent variables, and the program will fill the corresponding content into the request at the time of the request.

The available placeholders are as follows: 

| Placeholder     | Description                                                  |
| --------------- | ------------------------------------------------------------ |
| ${sender}       | Will be replaced with the AI name or user name based on the message's sender |
| ${content}      | Will be replaced with the message content                    |
| ${user_sender}  | Will be replaced with the user name, cannot be used with `${sender}` |
| ${ai_sender}    | Will be replaced with the AI name, cannot be used with `${sender}` |
| ${user_content} | Will be replaced with the user's message content, cannot be used with `${content}` |
| ${ai_content}   | Will be replaced with the AI's message content, cannot be used with `${content}` |
| ${history}      | Will be replaced with text composed of alternating `historyTemplate` and `,` |
| ${prompt}       | Will be replaced with the content last sent by the user      |

> **You can submit this part of the content to the AI and send the API request and response examples you want to use to the AI, so that the AI can help you generate Json configuration information.**

## TODO

- [ ] Determine the chat interface expansion direction. 
- [x] Custom API. 
- [ ] Right-click to delete a message.
- [ ] Personalize appearance.
- [ ] User Customized Chat Template. 
## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
