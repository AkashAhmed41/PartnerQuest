export interface Message {
    id: number
    senderId: number
    senderUsername: string
    senderProfilePhotoUrl: string
    recipientId: number
    recipientUsername: string
    recipientProfilePhotoUrl: string
    messageContent: string
    messageRead?: Date
    messageSent: Date
  }