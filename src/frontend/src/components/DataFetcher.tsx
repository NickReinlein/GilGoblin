const itemPath = `item/`;
const getBaseUrl = `http://localhost:55448/`;
const fetchData = async (tabName: string, id: number, world: number) => {
    try {
        let suffix = '';
        switch (tabName) {
            case 'Items':
                suffix = `${itemPath}${id}`;
                break;
            case 'Recipes':
            case 'Prices':
            case 'Crafting':
                suffix = '';
        }
        let url = `${getBaseUrl}${suffix}`;
        console.log(`Fetching from url ${url}`)
        const response = await fetch(url);
        return await response.json();
    } catch (error) {
        console.error('Error fetching data:', error);
    }
};

const DataFetcher = {
    fetchData,
};

export default DataFetcher;
