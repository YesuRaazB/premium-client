import axios from "axios";

// Replace with your API base URL
const API_BASE = "https://localhost:7191/api";

export const getOccupations = async () => {
    return axios.get(`${API_BASE}/occupations`);
};

export const calculatePremium = async (data) => {
    return axios.post(`${API_BASE}/members/calc`, data);
};
