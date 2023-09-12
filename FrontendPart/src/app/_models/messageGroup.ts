export interface MessageGroup {
    name: string;
    connections: Connection[];
}

export interface Connection {
    connectionId: string;
    username: string;
}