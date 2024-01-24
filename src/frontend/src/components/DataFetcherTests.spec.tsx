import fetchMock from 'jest-fetch-mock';
import DataFetcher, {DataFetcherType} from './DataFetcher';

beforeAll(() => {
    fetchMock.enableMocks();
});

afterEach(() => {
    fetchMock.resetMocks();
});

describe('DataFetcher', () => {
    const {fetchData} = DataFetcher as DataFetcherType;

    it('fetches data from the correct URL for Items', async () => {
        fetchMock.mockResponseOnce(JSON.stringify({data: 'mocked data'}));

        await fetchData('Items', 123, 456);

        expect.objectContaining({
            parsedURL: expect.objectContaining({
                href: 'http://localhost:55448/item/123',
            }),
            method: 'GET'
        });
    });

    it('fetches data from the correct URL for Recipes', async () => {
        fetchMock.mockResponseOnce(JSON.stringify({data: 'mocked data'}));

        await fetchData('Recipes', 789, 987);

        expect.objectContaining({
            parsedURL: expect.objectContaining({
                href: 'http://localhost:55448/recipe/789',
            }),
            method: 'GET'
        });
    });

    it('fetches data from the correct URL for Prices', async () => {
        fetchMock.mockResponseOnce(JSON.stringify({data: 'mocked data'}));

        await fetchData('Prices', 456, 789);

        expect.objectContaining({
            parsedURL: expect.objectContaining({
                href: 'http://localhost:55448/price/789/456',
            }),
            method: 'GET'
        });
    });

    it('fetches data from the correct URL for Crafts', async () => {
        fetchMock.mockResponseOnce(JSON.stringify({data: 'mocked data'}));

        await fetchData('Crafts', 987, 654);

        expect.objectContaining({
            parsedURL: expect.objectContaining({
                href: 'http://localhost:55448/crafts/987',
            }),
            method: 'GET'
        });
    });

    it('fetches data from the correct URL for Profits', async () => {
        fetchMock.mockResponseOnce(JSON.stringify({data: 'mocked data'}));

        await fetchData('Profits', 987, 654);

        expect.objectContaining({
            parsedURL: expect.objectContaining({
                href: 'http://localhost:55448/crafts/987/654',
            }),
            method: 'GET'
        });
    });

    it('handles errors gracefully', async () => {
        fetchMock.mockReject(new Error('Mocked error'));

        const tabName = 'UnknownTab';
        const id = 999;
        const world = 4;

        const result = await fetchData(tabName, id, world);

        expect(fetchMock).toHaveBeenCalled();
        expect(result).toBeUndefined();
    });
});
