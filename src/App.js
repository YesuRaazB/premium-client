import React, { useEffect, useState, useRef } from "react";
import "./App.css";

const SHOW_DEBUG = process.env.REACT_APP_DEBUG === "true";

function App() {
    const [occupations, setOccupations] = useState([]);
    const [form, setForm] = useState({
        name: "",
        ageNextBirthday: "",
        dob: "",
        occupationCode: "",
        deathSumInsured: ""
    });
    const [monthlyPremium, setMonthlyPremium] = useState(null);
    const [loadingPremium, setLoadingPremium] = useState(false);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState(null);

    // API base: prefer env var, fallback to same host with port or default 5000
    const apiBase =
        process.env.REACT_APP_API_BASE ||
        `${window.location.protocol}//${window.location.hostname}:${process.env.REACT_APP_API_PORT || window.location.port || 5000}`;

    const debounceRef = useRef(null);

    // --- Load occupations and normalise shapes ---
    useEffect(() => {
        let mounted = true;
        const url = `${apiBase}/api/occupations`;
        console.log("[occupations] fetching from", url);

        fetch(url)
            .then(async (r) => {
                if (!r.ok) {
                    const txt = await r.text().catch(() => null);
                    throw new Error(`Failed to load occupations (${r.status}) ${txt ? "- " + txt : ""}`);
                }
                return r.json();
            })
            .then((data) => {
                console.log("[occupations] raw response:", data);
                if (!mounted) return;

                // Normalise common shapes:
                //  - ["Doctor", "Farmer"]
                //  - [{ id:1, name:"Doctor", code:"Doctor" }, ...]
                //  - { success:true, data: [...] }
                let arr = data;
                if (data && typeof data === "object" && !Array.isArray(data) && Array.isArray(data.data)) {
                    arr = data.data;
                }
                if (!Array.isArray(arr)) arr = [];

                // Convert string items to object shape, and normalise object keys
                const normalised = arr.map((item, i) => {
                    if (typeof item === "string") {
                        return { _key: `s-${i}`, code: String(item), displayName: item };
                    }
                    // item is object — extract likely fields with fallbacks
                    const rawId = item.id ?? item.Id ?? item._id ?? item.key;
                    const code = item.code ?? item.Code ?? item.codeValue ?? rawId ?? item.name ?? item.Name;
                    const displayName =
                        item.displayName ?? item.name ?? item.Name ?? item.DisplayName ?? item.code ?? String(code ?? `#${i}`);
                    const key = rawId ?? item.code ?? `i-${i}`;
                    return { _key: String(key), code: String(code ?? displayName), displayName };
                });

                console.log("[occupations] normalised:", normalised);
                setOccupations(normalised);
            })
            .catch((err) => {
                console.error("load occupations error:", err);
                if (mounted) setOccupations([]);
            });

        return () => {
            mounted = false;
        };
    }, [apiBase]);

    // --- Calculate monthly premium when occupation, age or death sum changes ---
    useEffect(() => {
        const { occupationCode, deathSumInsured, ageNextBirthday } = form;

        // require all three to calculate
        if (!occupationCode || !deathSumInsured || !ageNextBirthday) {
            setMonthlyPremium(null);
            return;
        }

        // debounce
        if (debounceRef.current) clearTimeout(debounceRef.current);
        debounceRef.current = setTimeout(async () => {
            setLoadingPremium(true);
            setError(null);
            try {
                const qs = new URLSearchParams({
                    occupationCode,
                    death: parseFloat(deathSumInsured).toString(),
                    age: parseInt(ageNextBirthday, 10).toString()
                });

                const res = await fetch(`${apiBase}/api/members/calc?${qs.toString()}`, { method: "GET" });
                if (!res.ok) {
                    const txt = await res.text();
                    throw new Error(txt || `Calc failed (${res.status})`);
                }
                const data = await res.json();
                // support multiple possible property names from backend
                const mp = data.monthlyPremium ?? data.MonthlyPremium ?? data.monthlypremium ?? null;
                setMonthlyPremium(mp == null ? null : Number(mp));
            } catch (err) {
                console.error("calc error:", err);
                setMonthlyPremium(null);
                setError(err.message ?? "Calculation failed");
            } finally {
                setLoadingPremium(false);
            }
        }, 400);

        return () => {
            if (debounceRef.current) {
                clearTimeout(debounceRef.current);
                debounceRef.current = null;
            }
        };
    }, [apiBase, form]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        /* setForm((prev) => ({ ...prev, [name]: value }));*/

        setForm(prev => ({ ...prev, [name]: value === null ? "" : value }));
    };


    const handleSubmit = async (e) => {
        e.preventDefault();
        // basic validation
        if (!form.name || !form.ageNextBirthday || !form.dob || !form.occupationCode || !form.deathSumInsured) {
            alert("Please fill all fields.");
            return;
        }

        const payload = {
            name: form.name,
            ageNextBirthday: parseInt(form.ageNextBirthday, 10),
            dateOfBirthMMYYYY: form.dob,
            occupationCode: form.occupationCode,
            deathSumInsured: parseFloat(form.deathSumInsured)
        };

        try {
            setSaving(true);
            const res = await fetch(`${apiBase}/api/members`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            if (!res.ok) {
                const txt = await res.text();
                throw new Error(txt || `Save failed (${res.status})`);
            }

            const data = await res.json();
            const mp = data.monthlyPremium ?? data.MonthlyPremium ?? data.monthlypremium ?? null;
            alert(`Member saved. Monthly premium: ${formatCurrency(mp)}`);
        } catch (err) {
            console.error("save error:", err);
            alert("Error saving member: " + (err.message || "unknown"));
        } finally {
            setSaving(false);
        }
    };

    const formatCurrency = (value) => {
        if (value == null || isNaN(Number(value))) return "—";
        return new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 2 }).format(
            Number(value)
        );
    };

    return (
        <div style={{ maxWidth: 700, margin: "auto", padding: 20 }}>
            <h2>Insurance Premium Calculator</h2>

            <form onSubmit={handleSubmit} aria-describedby="form-note">
                <div style={{ marginBottom: 12 }}>
                    <label htmlFor="name">Name</label>
                    <br />
                    <input id="name" name="name" value={form.name} onChange={handleChange} />
                </div>

                <div style={{ marginBottom: 12 }}>
                    <label htmlFor="ageNextBirthday">Age Next Birthday</label>
                    <br />
                    <input
                        id="ageNextBirthday"
                        name="ageNextBirthday"
                        value={form.ageNextBirthday}
                        onChange={handleChange}
                        type="number"
                        min="0"
                    />
                </div>

                <div style={{ marginBottom: 12 }}>
                    <label htmlFor="dob">Date of Birth (MM/YYYY)</label>
                    <br />
                    <input id="dob" name="dob" value={form.dob} onChange={handleChange} placeholder="MM/YYYY" />
                </div>

                <div style={{ marginBottom: 12 }}>
                    <label htmlFor="occupationCode">Occupation</label>
                    <br />
                    <select id="occupationCode" name="occupationCode" value={String(form.occupationCode ?? "")} onChange={handleChange}>
                        <option value="">-- Select --</option>
                        {occupations.map((o) => (
                            <option key={o._key ?? o.code ?? JSON.stringify(o)} value={String(o.code ?? o._key)}>
                                {o.displayName ?? o.name ?? o.code}
                            </option>
                        ))}
                    </select>

                    {SHOW_DEBUG && (
                        <div style={{ marginTop: 8 }}>
                            <small>Debug occupations:</small>
                            <pre style={{ maxHeight: 120, overflow: "auto", background: "#f6f6f6", padding: 6 }}>
                                {JSON.stringify(occupations, null, 2)}
                            </pre>
                        </div>
                    )}
                </div>

                <div style={{ marginBottom: 12 }}>
                    <label htmlFor="deathSumInsured">Death Sum Insured</label>
                    <br />
                    <input
                        id="deathSumInsured"
                        name="deathSumInsured"
                        value={form.deathSumInsured}
                        onChange={handleChange}
                        type="number"
                        step="0.01"
                        min="0"
                    />
                </div>

                <div style={{ marginTop: 10 }}>
                    <button type="submit" disabled={saving}>
                        {saving ? "Saving..." : "Save Member"}
                    </button>
                </div>
            </form>

            <div style={{ marginTop: 20 }}>
                <h3>Monthly Premium</h3>
                <div style={{ fontSize: 20, fontWeight: "bold" }}>
                    {loadingPremium ? "Calculating..." : monthlyPremium !== null ? formatCurrency(monthlyPremium) : "—"}
                </div>
                {error && <div style={{ color: "crimson" }}>Error: {error}</div>}
                <small id="form-note">
                Note: premium updates when you change Occupation, Age and Death Sum Insured (auto-calculated with a short debounce).
                </small>
            </div>
        </div>
    );
}

export default App;
