import React, { useEffect, useState } from 'react';
import api from '../services/api';


export default function PremiumForm() {
    const [occupations, setOccupations] = useState([]);
    const [form, setForm] = useState({ name: '', ageNextBirthday: '', dob: '', occupationCode: '', sumInsured: 100000 });
    const [result, setResult] = useState(null);


    useEffect(() => { api.get('/members/occupations').then(r => setOccupations(r.data)).catch(() => { }); }, []);


    const submit = async (e) => {
        e.preventDefault();
        try {
            const res = await api.post('/members/calculate', {
                AgeNextBirthday: parseInt(form.ageNextBirthday, 10),
                SumInsured: parseFloat(form.sumInsured),
                OccupationCode: form.occupationCode
            });
            setResult(res.data.monthly);
        } catch (err) {
            alert('Calculation failed');
        }
    }
}