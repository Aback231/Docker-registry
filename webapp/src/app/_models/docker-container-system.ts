export interface DockerContainerSystem {
    id: string
    image: string
    imageID: string
    command: string
    created: string
    state: string
    status: string
}