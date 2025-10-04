import ENDPOINT__URLS from "../constants/endpoints";

const MessagingService = {
  async getChatByParticipantIds(participantIds, userToken) {
    return await fetch(ENDPOINT__URLS.START_CHAT, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${userToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(participantIds),
    })
      .then((response) => response.json())
      .then((requestResult) => requestResult)
      .catch((error) => console.log(error));
  },
};

export default MessagingService;
