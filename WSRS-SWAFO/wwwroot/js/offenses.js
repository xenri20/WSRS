const fetchAllDescriptions = async () => {
    const url = '/api/Offenses/GetAllDescriptions';

    try {
        const request = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
            }
        });

        const response = await request.json();
        return response;

    } catch (error) {
        console.error(new Error(error.message));
        alert('Something went wrong');
    }
}

const loadDescriptionSuggestions = async () => {
    const suggestions = await fetchAllDescriptions();

    if (!suggestions) return;

    const suggestionsList = document.getElementById("descriptionSuggestions");

    for (let s of suggestions) {
        const option = document.createElement("option");
        option.setAttribute("value", s);
        suggestionsList.append(option);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadDescriptionSuggestions();
});
