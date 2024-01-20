import React, {useEffect, useState} from 'react';

interface TimestampToAgeConverterProps {
    timestamp: string;
}

const TimestampToAgeConverter: React.FC<TimestampToAgeConverterProps> = ({timestamp}) => {
    const [age, setAge] = useState('');

    useEffect(() => {
        const date = new Date(timestamp);
        const now = new Date();

        const ageInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

        if (ageInSeconds < 60) {
            setAge(`${ageInSeconds} seconds ago`);
        } else if (ageInSeconds < 3600) {
            const ageInMinutes = Math.floor(ageInSeconds / 60);
            setAge(`${ageInMinutes} minutes ago`);
        } else if (ageInSeconds < 86400) {
            const ageInHours = Math.floor(ageInSeconds / 3600);
            setAge(`${ageInHours} hours ago`);
        } else {
            const ageInDays = Math.floor(ageInSeconds / 86400);
            setAge(`${ageInDays.toLocaleString()} days ago`);
        }
    }, [timestamp]);

    return (
        <div>
            <p>{age}</p>
        </div>
    );
};

export default TimestampToAgeConverter;