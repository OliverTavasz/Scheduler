export default function TypeMap(type) {
    switch (type) {
        case 0:
            return "Music";
        case 1:
            return "Prerecorded";
        case 2:
            return "Live";
        default:
            return "Unkown";
    }
};