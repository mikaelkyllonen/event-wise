import axios from 'axios';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:12000';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to include auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Types
export interface Event {
  id: string;
  name: string;
  description: string;
  location: string;
  eventState: string;
  startTime: string;
  endTime?: string;
  createdAtUtc: string;
}

export interface EventListItem {
  name: string;
  description: string;
  location: string;
  startTimeUtc: string;
  endTimeUtc?: string;
}

export interface CreateEventRequest {
  name: string;
  description: string;
  location: string;
  maxParticipants: number;
  startTime: string;
  endTime: string;
}

export interface UpdateEventRequest {
  name?: string;
  description?: string;
  location?: string;
  maxParticipants?: number;
  startTime?: string;
  endTime?: string;
}

// API functions
export const eventApi = {
  // Get all events
  getEvents: async (): Promise<{ events: EventListItem[] }> => {
    const response = await api.get('/events');
    return response.data;
  },

  // Get single event
  getEvent: async (id: string): Promise<Event> => {
    const response = await api.get(`/events/${id}`);
    return response.data;
  },

  // Create event (requires event_organizer role)
  createEvent: async (event: CreateEventRequest): Promise<{ id: string }> => {
    const response = await api.post('/events', event);
    return response.data;
  },

  // Update event (requires event_organizer role)
  updateEvent: async (id: string, event: UpdateEventRequest): Promise<{ id: string }> => {
    const response = await api.put(`/events/${id}`, event);
    return response.data;
  },

  // Join event
  joinEvent: async (eventId: string): Promise<void> => {
    await api.post(`/events/${eventId}/participants`);
  },

  // Leave event
  leaveEvent: async (eventId: string): Promise<void> => {
    await api.delete(`/events/${eventId}/participants`);
  },
};

export const userApi = {
  // Register user
  registerUser: async (user: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
  }): Promise<void> => {
    await api.post('/users', user);
  },
};

export default api;