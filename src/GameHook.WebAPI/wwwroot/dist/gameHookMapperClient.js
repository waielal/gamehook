class GameHookProperty {
    _client = null

    path = null
    address = null
    length = null
    value = null
    bytes = null
    frozen = null

    constructor(client, obj) {
        this._client = client

        for (const item of Object.entries(obj)) {
            this[item[0]] = item[1]
        }
    }

    async set(value, freeze) { this._client._editPropertyValue(this.path, value, freeze) }
    async setBytes(bytes, freeze) { this._client._editPropertyBytes(this.path, bytes, freeze) }

    async freeze(freeze = true) {
        if (freeze == true) {
            this._client._editPropertyBytes(this.path, this.bytes, freeze)
        } else if (freeze == false) {
            this._client._editPropertyBytes(this.path, null, false)
        }
    }

    change(fn) {
        if (!this._client._change[this.path]) {
            this._client._change[this.path] = []
        }

        this._client._change[this.path].push(fn)
    }

    once(fn) {
        if (!this._client._once[this.path]) {
            this._client._once[this.path] = []
        }

        this._client._once[this.path].push(fn)
    }

    toString() {
        if (this.value === undefined || this.value === null) { return null }

        return this.value.toString()
    }
}

class GameHookMapperClient {
    _connectionString
    _signalrClient
    meta
    _properties
    properties
    glossary

    _change = []
    _once = []

    static integerToHexdecimalString(x, uppercase = true) {
        if (x == null) return null

        let stringValue = x.toString(16)

        // If the string is of odd length, we
        // need to introduce a leading zero.
        if (stringValue.length % 2) {
            stringValue = '0' + stringValue
        }

        // Add a space after every 2 characters.
        stringValue = stringValue.replace(/.{1,2}(?=(.{2})+$)/g, '$& ')

        if (uppercase) return stringValue.toUpperCase()
        else return stringValue
    }

    static hexdecimalStringToInteger(x) {
        if (x == null) return null

        return parseInt(x.replace(' ', ''), 16)
    }

    constructor(connectionString = 'http://localhost:8085') {
        this._connectionString = connectionString

        this._options = {
            automaticRefreshMapperTimeMinutes: 1
        }
    }

    get _signalrConnectionEstablished() {
        return this._signalrClient != null && this._signalrClient.connection.q === 'Connected'
    }

    _deconstructMapper() {
        this.meta = null
        this._properties = null
        this.properties = null
        this.glossary = null
    }

    async loadMapper() {
        console.debug('[GameHook Client] Loading mapper.')

        function assign(final, path, value) {
            let lastKeyIndex = path.length - 1

            for (var i = 0; i < lastKeyIndex; ++i) {
                let key = path[i]
                if (!(key in final)) {
                    final[key] = /^\d+$/.test(path[i + 1]) ? [] : {}
                }

                final = final[key]
            }

            final[path[lastKeyIndex]] = value
        }

        let mapper = await fetch(`${this._connectionString}/mapper`)
            .then(async (x) => {
                return { response: x, body: await x.json() }
            })
            .then(x => {
                if (x.response.status === 200) {
                    return x.body
                } else {
                    this._deconstructMapper()

                    if (x.body) {
                        throw x.body
                    } else {
                        throw new Error('Unknown error.')
                    }
                }
            })

        this.meta = mapper.meta
        this.glossary = mapper.glossary

        // Translate properties from a flat array to a nested object.
        this.properties = {}
        this._properties = mapper.properties.map(x => new GameHookProperty(this, x))
        this._properties.forEach(x => assign(this.properties, x.path.split('.'), x))

        setTimeout(() => this.loadMapper(), this._options.automaticRefreshMapperTimeMinutes * 60000)

        return this
    }

    async _establishConnection() {
        try {
            if (this._signalrConnectionEstablished == false) {
                await this._signalrClient.start()
                console.debug('[GameHook Client] GameHook successfully established a SignalR connection.')
            }

            // Load the data from the server.
            await this.loadMapper()

            console.debug('[GameHook Client] GameHook is now connected.')
            this.onConnected()

            return true
        } catch (err) {
            this._deconstructMapper()

            console.error(err)
            this.onMapperLoadError(err)

            setTimeout(() => this._establishConnection(), 5000)

            return false
        }
    }

    async connect() {
        var that = this

        this._signalrClient = new signalR.HubConnectionBuilder()
            .withUrl(`${this._connectionString}/updates`)
            .configureLogging(signalR.LogLevel.Warning)
            .build()

        this._signalrClient.onclose(async () => {
            console.debug('[GameHook Client] SignalR connection lost. Attempting to reconnect...')

            this._deconstructMapper()
            this.onDisconnected()
            await this._establishConnection()
        })

        this._signalrClient.on('PropertyChanged', (path, value, bytes, frozen) => {
            if (that._properties && that._properties.length > 0) {
                let property = that._properties.find(x => x.path === path)
                if (!property) {
                    console.warn(`[GameHook Client] Could not find a related property in PropertyUpdated event for: ${path} ${value} ${bytes} ${frozen}`)
                    return
                }

                let oldProperty = { value: property.value, bytes: property.bytes }

                property.value = value
                property.bytes = bytes
                property.frozen = frozen

                // Trigger the property.change events if any.
                const changeArray = that._change[property.path]
                if (changeArray && changeArray.length > 0) {
                    changeArray.forEach(x => {
                        x(property, oldProperty)
                    })
                }

                // Trigger the property.once events if any.
                const onceArray = that._once[property.path]
                if (onceArray && onceArray.length > 0) {
                    onceArray.forEach(x => {
                        x(property, oldProperty)
                    })

                    that._once[property.path] = []
                }

                // Trigger the global property changed event.
                if (that.onPropertyChanged) {
                    that.onPropertyChanged(property, oldProperty)
                }
            } else {
                console.debug(`[GameHook Client] Mapper is not loaded, throwing away event. PropertyUpdated ${path} ${value} ${bytes} ${frozen}`)
            }
        })

        this._signalrClient.on('PropertyFrozen', (path) => {
            if (that._properties && that._properties.length > 0) {
                let property = that._properties.find(x => x.path === path)
                if (!property) {
                    console.warn(`[GameHook Client] Could not find a related property in PropertyUpdated event for: ${path} ${value} ${bytes}`)
                    return
                }

                property.frozen = true

                // Trigger the property changed event.
                if (property.onFrozen) {
                    property.onFrozen(property)
                }

                // Trigger the global property changed event.
                if (that.onPropertyFrozen) {
                    that.onPropertyFrozen(property)
                }
            } else {
                console.debug(`[GameHook Client] Mapper is not loaded, throwing away event. PropertyUpdated ${path} ${value} ${bytes}`)
            }
        })

        this._signalrClient.on('PropertyUnfrozen', (path) => {
            if (that._properties && that._properties.length > 0) {
                let property = that._properties.find(x => x.path === path)
                if (!property) {
                    console.warn(`[GameHook Client] Could not find a related property in PropertyUpdated event for: ${path} ${value} ${bytes}`)
                    return
                }

                property.frozen = false

                // Trigger the property changed event.
                if (property.onUnfrozen) {
                    property.onUnfrozen(property)
                }

                // Trigger the global property changed event.
                if (that.onPropertyUnfrozen) {
                    that.onPropertyUnfrozen(property)
                }
            } else {
                console.debug(`[GameHook Client] Mapper is not loaded, throwing away event. PropertyUpdated ${path} ${value} ${bytes}`)
            }
        })

        this._signalrClient.on('MapperLoaded', async () => { await this.loadMapper(); this.onMapperLoaded() })
        this._signalrClient.on('GameHookError', (err) => { this.onGameHookError(err) })
        this._signalrClient.on('DriverError', (err) => { this.onDriverError(err) })
        this._signalrClient.on('SendDriverRecovered', () => { this.onDriverRecovered() })

        return (await this._establishConnection())
    }

    async _editPropertyValue(path, value, freeze) {
        path = path.replace('.', '/')

        await fetch(`${this._connectionString}/mapper/properties/${path}/`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ value: value, freeze: freeze })
        })
            .then(async (x) => { return { response: x } })
            .then(x => {
                if (x.response.status === 200) {
                    return
                } else {
                    if (x.body) {
                        throw new Error(x.body)
                    } else {
                        throw new Error('Unknown error')
                    }
                }
            })
    }

    async _editPropertyBytes(path, bytes, freeze) {
        path = path.replace('.', '/')

        await fetch(`${this._connectionString}/mapper/properties/${path}/`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ bytes: bytes, freeze: freeze })
        })
            .then(async (x) => { return { response: x } })
            .then(x => {
                if (x.response.status === 200) {
                    return
                } else {
                    if (x.body) {
                        throw new Error(x.body)
                    } else {
                        throw new Error('Unknown error')
                    }
                }
            })
    }

    onConnected() { /* Override this with your own function. */ }
    onDisconnected() { /* Override this with your own function. */ }

    onGameHookError(err) { /* Override this with your own function. */ }
    onMapperLoaded() { /* Override this with your own function. */ }
    onMapperLoadError(err) { /* Override this with your own function. */ }
    onDriverError(err) { /* Override this with your own function. */ }
    onPropertyChanged(property) { /* Override this with your own function. */ }
    onPropertyFrozen(property) { /* Override this with your own function. */ }
    onPropertyUnfrozen(property) { /* Override this with your own function. */ }
}