import Link from "next/link";

export default function Home() {
  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        <header className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Welcome to EventWise
          </h1>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Discover and organize amazing events in your community. 
            Join existing events or create your own with our easy-to-use platform.
          </p>
        </header>

        <div className="grid md:grid-cols-2 gap-8 max-w-4xl mx-auto">
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">
              Discover Events
            </h2>
            <p className="text-gray-600 mb-6">
              Browse through a variety of events happening in your area. 
              Find something that interests you and join the community.
            </p>
            <Link
              href="/events"
              className="inline-block bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors"
            >
              View All Events
            </Link>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">
              Create Events
            </h2>
            <p className="text-gray-600 mb-6">
              Have an idea for an event? Create and manage your own events. 
              Bring people together around shared interests and activities.
            </p>
            <Link
              href="/events/create"
              className="inline-block bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 transition-colors"
            >
              Create Event
            </Link>
          </div>
        </div>

        <div className="mt-16 text-center">
          <h3 className="text-2xl font-semibold text-gray-900 mb-8">
            How It Works
          </h3>
          <div className="grid md:grid-cols-3 gap-8 max-w-4xl mx-auto">
            <div className="text-center">
              <div className="bg-blue-100 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <span className="text-2xl font-bold text-blue-600">1</span>
              </div>
              <h4 className="text-lg font-semibold text-gray-900 mb-2">
                Browse Events
              </h4>
              <p className="text-gray-600">
                Explore events in your area and find ones that match your interests.
              </p>
            </div>
            <div className="text-center">
              <div className="bg-green-100 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <span className="text-2xl font-bold text-green-600">2</span>
              </div>
              <h4 className="text-lg font-semibold text-gray-900 mb-2">
                Join or Create
              </h4>
              <p className="text-gray-600">
                Join existing events or create your own to bring people together.
              </p>
            </div>
            <div className="text-center">
              <div className="bg-purple-100 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
                <span className="text-2xl font-bold text-purple-600">3</span>
              </div>
              <h4 className="text-lg font-semibold text-gray-900 mb-2">
                Connect & Enjoy
              </h4>
              <p className="text-gray-600">
                Meet new people, share experiences, and enjoy amazing events.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
